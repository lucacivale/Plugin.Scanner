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
using Plugin.Scanner.Android.Views;

namespace Plugin.Scanner.Android.Barcode;

internal sealed class SingleBarcodeScannerDialog : AppCompatDialog, IConsumer
{
    private readonly AppCompatActivity _activity;

    private readonly LifecycleCameraController _cameraController;

    private Xamarin.Google.MLKit.Vision.BarCode.IBarcodeScanner _barcodeScanner;

    private TaskCompletionSource<string>? _scanCompleteTaskSource;

    public SingleBarcodeScannerDialog(AppCompatActivity context, int themeResId)
        : base(context, themeResId)
    {
        _activity = context;

        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.SingleBarcodeScanner);

        _cameraController = new(Context);
        _cameraController.BindToLifecycle(_activity);

        SetupOverlay();
    }

    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        _scanCompleteTaskSource = new();

        using BarcodeScannerOptions.Builder builder = new();
        using BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatAllFormats)
            .Build();

        _barcodeScanner = BarcodeScanning.GetClient(scannerOptions);

        IExecutor mainExecutor = ContextCompat.GetMainExecutor(Context) ?? throw new NullReferenceException();

        using MlKitAnalyzer analyzer = new([_barcodeScanner], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, this);

        _cameraController.SetImageAnalysisAnalyzer(mainExecutor, analyzer);

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        return new Core.Barcode.Barcode(barcode);
    }

    public override void Show()
    {
        PermissionsHelper.CheckPermissions(_activity);

        base.Show();
    }

    public override void Dismiss()
    {
        Cleanup();

        base.Dismiss();
    }

    public override void Cancel()
    {
        Cleanup();

        base.Cancel();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cameraController.Dispose();
            _barcodeScanner.Dispose();
        }

        base.Dispose(disposing);
    }

    private void Cleanup()
    {
        PreviewView previewView = FindViewById<PreviewView>(Resource.Id.previewView) ?? throw new NullReferenceException();
        previewView.Controller = null;
        previewView.Overlay?.Clear();

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(Resource.Id.barcodeButton) ?? throw new NullReferenceException();
        barcodeButton.Clicked -= BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(Resource.Id.closeButton) ?? throw new NullReferenceException();
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(Resource.Id.flashButton) ?? throw new NullReferenceException();
        flashButton.Toggled -= FlashButton_Toggled;

        _cameraController.ClearImageAnalysisAnalyzer();
        _cameraController.Unbind();

        _barcodeScanner.Close();
    }

    public void Accept(Java.Lang.Object? t)
    {
        if (t is not MlKitAnalyzer.Result result)
        {
            return;
        }

        PreviewView previewView = FindViewById<PreviewView>(Resource.Id.previewView) ?? throw new NullReferenceException();
        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(Resource.Id.barcodeButton) ?? throw new NullReferenceException();

        previewView.Overlay?.Clear();

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
                    throw new NullReferenceException();
                }

                previewView.Overlay?.Add(new BarcodeHighlight(barcode.BoundingBox));

                if (barcode.DisplayValue != barcodeButton.Barcode?.DisplayValue)
                {
                    barcodeButton.Barcode = barcode;
                    barcodeButton.Visibility = ViewStates.Visible;
                }
            }
        }
    }

    private void SetupOverlay()
    {
        PreviewView previewView = FindViewById<PreviewView>(Resource.Id.previewView) ?? throw new NullReferenceException();
        previewView.Controller = _cameraController;

        BarcodeItemButton barcodeButton = FindViewById<BarcodeItemButton>(Resource.Id.barcodeButton) ?? throw new NullReferenceException();
        barcodeButton.Clicked += BarcodeItemButtonClicked;

        ImageButton closeButton = FindViewById<ImageButton>(Resource.Id.closeButton) ?? throw new NullReferenceException();
        closeButton.Click += CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(Resource.Id.flashButton) ?? throw new NullReferenceException();
        flashButton.Toggled += FlashButton_Toggled;
    }

    private void FlashButton_Toggled(object? sender, int e)
    {
        _cameraController.ImageCaptureFlashMode = e;
        _cameraController.EnableTorch(e != ImageCapture.FlashModeOff);
    }

    private void BarcodeItemButtonClicked(object? sender, Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode e)
    {
        Dismiss();

        _scanCompleteTaskSource?.TrySetResult(e.DisplayValue);
    }

    private void CloseButtonClicked(object? sender, EventArgs e)
    {
        Cancel();

        _scanCompleteTaskSource?.TrySetResult(string.Empty);
    }
}
