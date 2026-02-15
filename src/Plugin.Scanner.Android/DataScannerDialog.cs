using AndroidX.AppCompat.App;
using AndroidX.Camera.Core;
using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Util;
using AndroidX.Lifecycle;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;

namespace Plugin.Scanner.Android;

internal abstract class DataScannerDialog<TResult> : AppCompatDialog, IConsumer
{
    private readonly Activity _activity;
    private readonly LifecycleCameraController _cameraController;

    private TaskCompletionSource<TResult>? _scanCompleteTaskSource;

    protected DataScannerDialog(Activity context)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_DataScannerDialog)
    {
        _activity = context;
        _cameraController = new(Context);

        if (_activity is not ILifecycleOwner)
        {
            throw new ActivityMustBeILifecycleOwnerException("Activity must implement ILifecycleOwner");
        }

        _cameraController.BindToLifecycle((ILifecycleOwner)_activity);

        SetContentView();
    }

    protected LifecycleCameraController CameraController => _cameraController;

    protected TaskCompletionSource<TResult>? ScanCompleteTaskSource => _scanCompleteTaskSource;

    public async Task<TResult> ScanAsync(CancellationToken cancellationToken)
    {
        if (Context.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        Show();

        _scanCompleteTaskSource = new();

        TResult result = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        // Wait here because MLKit won't stop analyzing until the pipeline is finished.
        // If we dispose the dialog before MLKit finishes, it will throw an exception.
        await Task.Delay(250, cancellationToken).ConfigureAwait(true);

        return result;
    }

    public void SetContentView()
    {
        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScanner);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        AddOverlay(FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));
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

    public void Accept(Java.Lang.Object? t)
    {
        if (t is not MlKitAnalyzer.Result result)
        {
            return;
        }

        Accept(result);
    }

    protected void SetFlashMode(int flashMode)
    {
        _cameraController.ImageCaptureFlashMode = flashMode;
        _cameraController.EnableTorch(flashMode != ImageCapture.FlashModeOff);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cameraController.Dispose();
        }

        base.Dispose(disposing);
    }

    protected virtual void Cleanup()
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;

        _cameraController.ClearImageAnalysisAnalyzer();
        _cameraController.Unbind();
    }

    protected abstract void AddOverlay(FrameLayout root);

    protected abstract void Accept(MlKitAnalyzer.Result result);
}
