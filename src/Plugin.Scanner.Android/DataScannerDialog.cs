using AndroidX.AppCompat.App;
using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android;

internal sealed class DataScannerDialog : AppCompatDialog
{
    private readonly Activity _activity;
    private readonly LifecycleCameraController _cameraController;
    private readonly IDataDetector _dataDetector;

    private readonly bool _recognizeMultiple;
    private readonly bool _isHighlightingEnabled;
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest? _regionOfInterest;

    private TaskCompletionSource<RecognizedItem>? _scanCompleteTaskSource;

    public DataScannerDialog(
        Activity context,
        IDataDetector detector,
        LifecycleCameraController cameraController,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_DataScannerDialog)
    {
        _activity = context;
        _dataDetector = detector;
        _cameraController = cameraController;
        _regionOfInterest = regionOfInterest;
        _overlay = overlay;

        _recognizeMultiple = recognizeMultiple;
        _isHighlightingEnabled = isHighlightingEnabled;

        SetContentView();
    }

    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    public EventHandler? Cleared { get; set; }

    public bool RecognizeMultiple => _recognizeMultiple;

    public bool IsHighlightingEnabled => _isHighlightingEnabled;

    public async Task<RecognizedItem> ScanAsync(CancellationToken cancellationToken)
    {
        if (Context.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        Show();

        _dataDetector.Detected += OnDetected;
        _dataDetector.Cleared += OnCleared;

        _scanCompleteTaskSource = new();

        RecognizedItem result = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        // Wait here because MLKit won't stop analyzing until the pipeline is finished.
        // If we dispose of the dialog before MLKit finishes, it will throw an exception.
        await Task.Delay(450, cancellationToken).ConfigureAwait(true);

        base.Dismiss();

        return result;
    }

    public override void Show()
    {
        PermissionsHelper.CheckPermissions(_activity);

        base.Show();
    }

    public override void Dismiss()
    {
        Cleanup();

        _scanCompleteTaskSource?.TrySetResult(RecognizedItem.Empty);
    }

    public void Dismiss(RecognizedItem item)
    {
        Cleanup();

        _scanCompleteTaskSource?.TrySetResult(item);
    }

    private void Cleanup()
    {
        _dataDetector.Stop();
        _dataDetector.Detected -= OnDetected;
        _dataDetector.Cleared -= OnCleared;

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;

        _overlay?.Cleanup();
    }

    private void SetContentView()
    {
        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScanner);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        _overlay?.Init(this, FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));
        _overlay?.AddOverlay();

        if (_regionOfInterest is not null)
        {
            EventHandler @event = null!;
            @event = (_, _) =>
            {
                FrameLayout frame = FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));

                _regionOfInterest?.SetConstraints(Convert.ToInt32(Context.FromPixels(frame.Width)), Convert.ToInt32(Context.FromPixels(frame.Height)));
                _dataDetector.RegionOfInterest = _regionOfInterest?.CalculateRegionOfInterest().ToRectPixel(Context);

                _overlay?.AddRegionOfInterest(_regionOfInterest);

                ShowEvent -= @event;
            };

            ShowEvent += @event;
        }
    }

    private void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        Detected?.Invoke(this, e);
    }

    private void OnCleared(object? sender, EventArgs e)
    {
        Cleared?.Invoke(this, EventArgs.Empty);
    }
}
