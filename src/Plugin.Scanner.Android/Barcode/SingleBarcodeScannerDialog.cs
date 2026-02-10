using AndroidX.AppCompat.App;
using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Core.Util;
using Java.Util;
using Java.Util.Concurrent;
using Plugin.Scanner.Core.Barcode;
using Xamarin.Google.MLKit.Vision.BarCode;
using Plugin.Scanner.Android.Barcode.Views;
using Java.Interop;
using AndroidX.Camera.Core;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Views;

namespace Plugin.Scanner.Android.Barcode;

/// <summary>
/// Represents a full-screen dialog that provides barcode scanning functionality using Google ML Kit and CameraX.
/// </summary>
/// <remarks>
/// <para>
/// This dialog displays a camera preview with real-time barcode detection, visual highlighting of detected barcodes,
/// and UI controls for flash, closing, and barcode selection.
/// </para>
/// <para>
/// The dialog implements <see cref="IConsumer"/> to receive ML Kit analysis results asynchronously
/// as the camera processes frames.
/// </para>
/// </remarks>
internal sealed class SingleBarcodeScannerDialog : AppCompatDialog, IConsumer
{
    private readonly AppCompatActivity _activity;
    private readonly LifecycleCameraController _cameraController;
    private readonly Xamarin.Google.MLKit.Vision.BarCode.IBarcodeScanner _barcodeScanner;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Objects are disposed when barcode is removed or when the scanner is closed.")]
    private BarcodeHighlight? _barcodeHighlight;

    private TaskCompletionSource<string>? _scanCompleteTaskSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleBarcodeScannerDialog"/> class.
    /// </summary>
    /// <param name="context">The parent activity context.</param>
    /// <param name="barcodeFormats">The collection of ML Kit barcode format identifiers to recognize.</param>
    /// <remarks>
    /// This constructor sets up the camera controller, binds it to the activity lifecycle,
    /// configures the dialog's content view, and initializes the ML Kit barcode analyzer
    /// with the specified formats.
    /// </remarks>
    public SingleBarcodeScannerDialog(AppCompatActivity context, IEnumerable<int> barcodeFormats)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_SingleBarcodeScanner)
    {
        _activity = context;
        _cameraController = new(Context);
        _cameraController.BindToLifecycle(_activity);

        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.SingleBarcodeScanner);
        SetOverlay();
        _barcodeScanner = SetAnalyzer(barcodeFormats);
    }

    /// <summary>
    /// Asynchronously displays the scanner dialog and waits for a barcode to be scanned or the operation to be canceled.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the scan operation.</param>
    /// <returns>
    /// A task that represents the asynchronous scan operation. The task result contains
    /// the scanned barcode.
    /// </returns>
    /// <exception cref="NoCameraException">Thrown when the device has no camera available.</exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled via the <paramref name="cancellationToken"/>
    /// or by the user closing the dialog.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The method shows the dialog, waits for the user to tap a detected barcode or close the dialog,
    /// then dismisses the dialog, and includes a 250ms delay to allow ML Kit's analysis pipeline to complete
    /// before disposing resources.
    /// </para>
    /// <para>
    /// If the user closes the dialog without scanning, an empty string is returned in the barcode value.
    /// </para>
    /// </remarks>
    public async Task<IBarcode> ScanAsync(CancellationToken cancellationToken)
    {
        if (_cameraController.HasCamera(_cameraController.CameraSelector) == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        Show();

        _scanCompleteTaskSource = new();

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        Dismiss();

        // Wait here because MLKit won't stop analyzing until the pipeline is finished.
        // If we dispose the dialog before MLKit finishes, it will throw an exception.
        await Task.Delay(250, cancellationToken).ConfigureAwait(true);

        return new Core.Barcode.Barcode(barcode);
    }

    /// <summary>
    /// Shows the scanner dialog after verifying camera permissions.
    /// </summary>
    /// <remarks>
    /// This method checks that the required camera permissions are granted before displaying the dialog.
    /// If permissions are not granted, it will request them or throw an exception.
    /// </remarks>
    public override void Show()
    {
        PermissionsHelper.CheckPermissions(_activity);

        base.Show();
    }

    /// <summary>
    /// Dismisses the scanner dialog and cleans up resources.
    /// </summary>
    /// <remarks>
    /// This method unsubscribes from event handlers, unbinds the camera controller,
    /// closes the barcode scanner, and removes any barcode highlights before dismissing the dialog.
    /// </remarks>
    public override void Dismiss()
    {
        Cleanup();

        base.Dismiss();
    }

    /// <summary>
    /// Accepts and processes ML Kit analysis results from the barcode scanner.
    /// </summary>
    /// <param name="t">The analysis result object from ML Kit.</param>
    /// <remarks>
    /// <para>
    /// This method is called by ML Kit's analyzer for each camera frame that is processed.
    /// It updates the UI to show or hide the barcode button and highlight based on whether
    /// a barcode is detected.
    /// </para>
    /// <para>
    /// When a barcode is detected, it:
    /// <list type="bullet">
    /// <item><description>Adds a visual highlight around the barcode</description></item>
    /// <item><description>Updates and shows the barcode button with the decoded value</description></item>
    /// </list>
    /// When no barcode is detected, it hides the button and removes the highlight.
    /// </para>
    /// </remarks>
    /// <exception cref="ViewNotFoundException">Thrown when required UI views cannot be found.</exception>
    /// <exception cref="MlKitAnalyzerResultNotBarcodeException">
    /// Thrown when ML Kit returns a result that is not a barcode object.
    /// </exception>
    public void Accept(Java.Lang.Object? t)
    {
        if (t is not MlKitAnalyzer.Result result)
        {
            return;
        }

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));

        RemoveHighlight(previewView.Overlay);

        ArrayList? results = null;
        result.GetValue(_barcodeScanner)?.TryJavaCast(out results);

        using (results)
        {
            if (results is null
                || results.IsEmpty)
            {
                barcodeButton.Visibility = ViewStates.Gone;
                barcodeButton.Barcode = null;
            }
            else
            {
                if (results.Get(0) is not Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode barcode)
                {
                    throw new MlKitAnalyzerResultNotBarcodeException("Result is not a barcode.");
                }

                AddHighlight(previewView.Overlay, barcode);

                if (barcode.DisplayValue != barcodeButton.Barcode?.DisplayValue)
                {
                    barcodeButton.Barcode = barcode;
                    barcodeButton.Visibility = ViewStates.Visible;
                }
            }
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SingleBarcodeScannerDialog"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cameraController.Dispose();
            _barcodeScanner.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Configures and returns the ML Kit barcode analyzer with the specified barcode formats.
    /// </summary>
    /// <param name="barcodeFormats">The collection of barcode format identifiers to recognize.</param>
    /// <returns>
    /// The configured <see cref="Xamarin.Google.MLKit.Vision.BarCode.IBarcodeScanner"/> instance
    /// that can be used to scan barcodes with the specified formats.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    /// <item><description>Creates a <see cref="BarcodeScannerOptions"/> with the specified formats</description></item>
    /// <item><description>Initializes a barcode scanner client with those options</description></item>
    /// <item><description>Creates an <see cref="MlKitAnalyzer"/> that uses view-referenced coordinates</description></item>
    /// <item><description>Attaches the analyzer to the camera controller's image analysis pipeline</description></item>
    /// <item><description>Returns the configured scanner for external use</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The first format in the collection is used as the primary format, with remaining formats
    /// passed as additional formats to recognize.
    /// </para>
    /// <para>
    /// The analyzer runs on the main executor and delivers results to this dialog via the
    /// <see cref="IConsumer"/> interface implementation.
    /// </para>
    /// </remarks>
    /// <exception cref="MainExecutorNotAvailableException">
    /// Thrown when the Android main executor is not available.
    /// </exception>
    private Xamarin.Google.MLKit.Vision.BarCode.IBarcodeScanner SetAnalyzer(IEnumerable<int> barcodeFormats)
    {
        List<int> formats = barcodeFormats.ToList();

        using BarcodeScannerOptions.Builder builder = new();
        using BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
            .Build();

        Xamarin.Google.MLKit.Vision.BarCode.IBarcodeScanner scanner = BarcodeScanning.GetClient(scannerOptions);

        IExecutor mainExecutor = ContextCompat.GetMainExecutor(Context) ?? throw new MainExecutorNotAvailableException("Main executor not available.");

        using MlKitAnalyzer analyzer = new([_barcodeScanner], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, this);

        _cameraController.SetImageAnalysisAnalyzer(mainExecutor, analyzer);

        return scanner;
    }

    /// <summary>
    /// Cleans up camera resources, event handlers, and UI elements.
    /// </summary>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Unbinds the camera preview controller</description></item>
    /// <item><description>Removes barcode highlights</description></item>
    /// <item><description>Unsubscribes from all UI control event handlers</description></item>
    /// <item><description>Clears and unbinds the camera analyzer</description></item>
    /// <item><description>Closes the ML Kit barcode scanner</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ViewNotFoundException">Thrown when required UI views cannot be found.</exception>
    private void Cleanup()
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;
        RemoveHighlight(previewView.Overlay);

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));
        barcodeButton.Clicked -= BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled -= FlashButton_Toggled;

        _cameraController.ClearImageAnalysisAnalyzer();
        _cameraController.Unbind();

        _barcodeScanner.Close();
    }

    /// <summary>
    /// Initializes the dialog's UI overlay controls and event handlers.
    /// </summary>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Binds the camera controller to the preview view</description></item>
    /// <item><description>Subscribes to the barcode button's clicked event</description></item>
    /// <item><description>Subscribes to the close button's click event</description></item>
    /// <item><description>Shows/hides the flash button based on flash unit availability</description></item>
    /// <item><description>Subscribes to the flash button's toggled event</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ViewNotFoundException">Thrown when required UI views cannot be found.</exception>
    private void SetOverlay()
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));
        barcodeButton.Clicked += BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click += CloseButtonClicked;
        closeButton.Visibility = _cameraController.CameraInfo?.HasFlashUnit == false ? ViewStates.Gone : ViewStates.Visible;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled += FlashButton_Toggled;
    }

    /// <summary>
    /// Handles the flash button toggle event to enable or disable the camera flash/torch.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The flash mode value (e.g., on, off, auto).</param>
    private void FlashButton_Toggled(object? sender, int e)
    {
        _cameraController.ImageCaptureFlashMode = e;
        _cameraController.EnableTorch(e != ImageCapture.FlashModeOff);
    }

    /// <summary>
    /// Handles the barcode button click event to complete the scan operation with the selected barcode.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The barcode object that was clicked.</param>
    /// <remarks>
    /// This method completes the scan task with the barcode's display value, triggering the dialog dismissal.
    /// If the barcode has no display value, a fallback message is used.
    /// </remarks>
    private void BarcodeItemButtonClicked(object? sender, Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode e)
    {
        _scanCompleteTaskSource?.TrySetResult(e.DisplayValue ?? "No display value found.");
    }

    /// <summary>
    /// Handles the close button click event to cancel the scan operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    /// <remarks>
    /// This method completes the scan task with an empty string, indicating the user canceled the operation.
    /// </remarks>
    private void CloseButtonClicked(object? sender, EventArgs e)
    {
        _scanCompleteTaskSource?.TrySetResult(string.Empty);
    }

    /// <summary>
    /// Removes the current barcode highlight from the overlay.
    /// </summary>
    /// <param name="overlay">The view overlay to clear the highlight from.</param>
    private void RemoveHighlight(ViewOverlay? overlay)
    {
        if (_barcodeHighlight is not null)
        {
            overlay?.Clear();
            _barcodeHighlight.Dispose();
        }
    }

    /// <summary>
    /// Adds a visual highlight around the detected barcode on the overlay.
    /// </summary>
    /// <param name="overlay">The view overlay to add the highlight to.</param>
    /// <param name="barcode">The detected barcode containing the bounding box coordinates.</param>
    /// <remarks>
    /// If the barcode has no bounding box information, no highlight is added.
    /// </remarks>
    private void AddHighlight(ViewOverlay? overlay, Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode barcode)
    {
        if (barcode.BoundingBox is not null)
        {
            _barcodeHighlight = new BarcodeHighlight(barcode.BoundingBox);
            overlay?.Add(_barcodeHighlight);
        }
    }
}
