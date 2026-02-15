using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Core.Util;
using Java.Util;
using Java.Util.Concurrent;
using Xamarin.Google.MLKit.Vision.BarCode;
using Plugin.Scanner.Android.Barcode.Views;
using Java.Interop;
using AndroidX.Camera.Core;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Views;
using Plugin.Scanner.Android.Extensions;

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
internal sealed class BarcodeScannerDialog : DataScannerDialog<Core.Barcode.Barcode>, View.IOnTouchListener
{
    private readonly IBarcodeScanner _barcodeScanner;
    private readonly bool _recognizeMultiple;
    private readonly List<BarcodeHighlight> _barcodeHighlights = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScannerDialog"/> class.
    /// </summary>
    /// <param name="context">The parent activity context.</param>
    /// <param name="barcodeFormats">The collection of ML Kit barcode format identifiers to recognize.</param>
    /// <remarks>
    /// This constructor sets up the camera controller, binds it to the activity lifecycle,
    /// configures the dialog's content view, and initializes the ML Kit barcode analyzer
    /// with the specified formats.
    /// </remarks>
    public BarcodeScannerDialog(Activity context, IEnumerable<int> barcodeFormats, bool recognizeMultiple)
        : base(context)
    {
        _recognizeMultiple = recognizeMultiple;

        if (_recognizeMultiple)
        {
            PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
            previewView.SetOnTouchListener(this);
        }

        List<int> formats = barcodeFormats.ToList();

        using BarcodeScannerOptions.Builder builder = new();
        using BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
            .Build();

        _barcodeScanner = BarcodeScanning.GetClient(scannerOptions);

        IExecutor mainExecutor = ContextCompat.GetMainExecutor(Context) ?? throw new MainExecutorNotAvailableException("Main executor not available.");

        using MlKitAnalyzer analyzer = new([_barcodeScanner], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, this);

        CameraController.SetImageAnalysisAnalyzer(mainExecutor, analyzer);
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
        base.Dismiss();

        ScanCompleteTaskSource?.TrySetResult(Core.Barcode.Barcode.Empty());
    }

    public override void Cancel()
    {
        base.Cancel();

        ScanCompleteTaskSource?.TrySetResult(Core.Barcode.Barcode.Empty());
    }

    public bool OnTouch(View? v, MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Up
            && _barcodeHighlights.FirstOrDefault(x => x.Barcode.BoundingBox?.Contains((int)e.GetX(), (int)e.GetY()) == true) is BarcodeHighlight highlight)
        {
            BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));

            if (highlight.Barcode.DisplayValue != barcodeButton.Barcode?.DisplayValue)
            {
                barcodeButton.Barcode = highlight.Barcode;
                barcodeButton.Visibility = ViewStates.Visible;
            }
        }

        return true;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BarcodeScannerDialog"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _barcodeScanner.Dispose();
        }

        base.Dispose(disposing);
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
    protected override void Cleanup()
    {
        base.Cleanup();

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.SetOnTouchListener(this);

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));
        barcodeButton.Clicked -= BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled -= FlashButton_Toggled;

        _barcodeScanner.Close();
    }

    protected override void AddOverlay(FrameLayout root)
    {
        LayoutInflater.Inflate(_Microsoft.Android.Resource.Designer.Resource.Layout.BarcodeScannerOverlay, root);

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));
        barcodeButton.Clicked += BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click += CloseButtonClicked;
        closeButton.Visibility = Context.HasFlash() == false ? ViewStates.Gone : ViewStates.Visible;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled += FlashButton_Toggled;
    }

    protected override void Accept(MlKitAnalyzer.Result result)
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.barcodeButton) ?? throw new ViewNotFoundException(nameof(BarcodeItemButton));

        RemoveHighlights(previewView.Overlay);

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
                List<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode> barcodes = results
                    .ToEnumerable()
                    .OfType<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>()
                    .ToList();

                if (barcodes.Count != 0)
                {
                    if (_recognizeMultiple == false)
                    {
                        Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode barcode = barcodes.First();
                        AddHighlight(previewView.Overlay, barcodes.First());

                        if (barcode.DisplayValue != barcodeButton.Barcode?.DisplayValue)
                        {
                            barcodeButton.Barcode = barcode;
                            barcodeButton.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        foreach (Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode barcode in barcodes)
                        {
                            AddHighlight(previewView.Overlay, barcode);
                        }

                        if (_barcodeHighlights.Any(x => barcodeButton.Text == x.Barcode.RawValue) == false)
                        {
                            barcodeButton.Barcode = null;
                            barcodeButton.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles the flash button toggle event to enable or disable the camera flash/torch.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The flash mode value (e.g., on, off, auto).</param>
    private void FlashButton_Toggled(object? sender, int e)
    {
        SetFlashMode(e);
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
        ScanCompleteTaskSource?.TrySetResult(new Core.Barcode.Barcode(e.DisplayValue ?? "No display value found."));
        Dismiss();
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
        Cancel();
    }

    /// <summary>
    /// Removes the current barcode highlight from the overlay.
    /// </summary>
    /// <param name="overlay">The view overlay to clear the highlight from.</param>
    private void RemoveHighlights(ViewOverlay? overlay)
    {
        overlay?.Clear();
        _barcodeHighlights.Clear();
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
            BarcodeHighlight highlight = new(barcode);

            _barcodeHighlights.Add(highlight);
            overlay?.Add(highlight);
        }
    }
}
