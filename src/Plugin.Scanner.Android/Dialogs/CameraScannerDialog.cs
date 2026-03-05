using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Dialogs;

internal sealed class CameraScannerDialog : DataScannerDialog
{
    private readonly LifecycleCameraController _cameraController;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraScannerDialog"/> class.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="detector">The data detector to use for recognition.</param>
    /// <param name="cameraController">The camera controller for managing camera operations.</param>
    /// <param name="regionOfInterest">Optional region of interest to limit scanning area.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    /// <param name="recognizeMultiple">Whether to recognize multiple items.</param>
    /// <param name="isHighlightingEnabled">Whether to highlight detected items.</param>
    public CameraScannerDialog(
        Activity context,
        IDataDetector detector,
        LifecycleCameraController cameraController,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context, detector, regionOfInterest, overlay, recognizeMultiple, isHighlightingEnabled)
    {
        _cameraController = cameraController;
    }

    /// <summary>
    /// Starts the scanning operation and waits for a recognized item.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous scan operation, containing the recognized item.</returns>
    /// <exception cref="NoCameraException">Thrown when the device has no camera.</exception>
    public override async Task<RecognizedItem> ScanAsync(CancellationToken cancellationToken)
    {
        if (Context.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        return await base.ScanAsync(cancellationToken).ConfigureAwait(true);
    }

    protected override void Cleanup()
    {
        base.Cleanup();

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;
    }

    protected override void SetContentView()
    {
        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.CameraScanner);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        base.SetContentView();
    }
}
