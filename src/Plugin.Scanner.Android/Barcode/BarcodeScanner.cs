using AndroidX.AppCompat.App;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Lifecycle;
using Google.Common.Util.Concurrent;
using Plugin.Scanner.Core.Barcode;
using Object = Java.Lang.Object;

namespace Plugin.Scanner.Android.Barcode;

internal sealed class BarcodeScanner : IBarcodeScanner
{
    private readonly ICurrentActivity _currentActivity;

    public BarcodeScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        Activity activity = _currentActivity.GetActivity();

        _ = activity ?? throw new ArgumentNullException(nameof(options));

        AppCompatDialog scannerDialog = new(activity, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_SingleBarcodeScanner);
        scannerDialog.SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.SingleBarcodeScanner);

        PreviewView? previewView = scannerDialog.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView);

        IListenableFuture cameraProvider = ProcessCameraProvider.GetInstance(activity);
        ProcessCameraProvider? cameraProviderInstance = (ProcessCameraProvider)cameraProvider.Get();

        Preview preview = new Preview.Builder().Build();
        preview.SetSurfaceProvider(previewView.SurfaceProvider);

        scannerDialog.Show();

        await Task.Delay(1000);
        
        previewView.Post(() =>
        {
            cameraProviderInstance?.BindToLifecycle(
                (ILifecycleOwner)activity,
                CameraSelector.DefaultBackCamera,
                preview);
        });

        return new Core.Barcode.Barcode(string.Empty);
    }
}