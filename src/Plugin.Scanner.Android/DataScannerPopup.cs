using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Controllers;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android;

internal sealed class DataScannerPopup : FrameLayout, IDataScannerController
{
    private readonly LifecycleCameraController _cameraController;
    private readonly IDataDetector _dataDetector;

    private readonly bool _recognizeMultiple;
    private readonly bool _isHighlightingEnabled;
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest? _regionOfInterest;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerPopup"/> class.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="detector">The data detector to use for recognition.</param>
    /// <param name="cameraController">The camera controller for managing camera operations.</param>
    /// <param name="regionOfInterest">Optional region of interest to limit scanning area.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    /// <param name="recognizeMultiple">Whether to recognize multiple items.</param>
    /// <param name="isHighlightingEnabled">Whether to highlight detected items.</param>
    public DataScannerPopup(
        Context context,
        IDataDetector detector,
        LifecycleCameraController cameraController,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context)
    {
        _dataDetector = detector;
        _cameraController = cameraController;
        _regionOfInterest = regionOfInterest;
        _overlay = overlay;

        _recognizeMultiple = recognizeMultiple;
        _isHighlightingEnabled = isHighlightingEnabled;

        SetContentView();
    }

    /// <summary>
    /// Gets or sets when items are detected by the scanner.
    /// </summary>
    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    /// <summary>
    /// Gets or sets when the detection area is cleared.
    /// </summary>
    public EventHandler? Cleared { get; set; }

    /// <summary>
    /// Gets a value indicating whether multiple items can be recognized simultaneously.
    /// </summary>
    public bool RecognizeMultiple => _recognizeMultiple;

    /// <summary>
    /// Gets a value indicating whether detected items should be highlighted.
    /// </summary>
    public bool IsHighlightingEnabled => _isHighlightingEnabled;

    public bool IsRunning => IsAttachedToWindow;

    public void Cancel()
    {
        if (Parent is ViewGroup parent)
        {
            parent.RemoveView(this);
        }
    }

    public void Dismiss(RecognizedItem item)
    {
        Cancel();
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();

        if (Context?.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        _dataDetector.Detected += OnDetected;
        _dataDetector.Cleared += OnCleared;
    }

    protected override void OnDetachedFromWindow()
    {
        base.OnDetachedFromWindow();

        Cleanup();
    }

    /// <summary>
    /// Cleans up scanner resources, detaches event handlers, and removes the overlay.
    /// </summary>
    private void Cleanup()
    {
        _dataDetector.Stop();
        _dataDetector.Detected -= OnDetected;
        _dataDetector.Cleared -= OnCleared;

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;

        _overlay?.Cleanup();
    }

    /// <summary>
    /// Initializes and configures the scanner dialog's content view with camera preview, overlay, and region of interest.
    /// </summary>
    private void SetContentView()
    {
        if (Context is null)
        {
            return;
        }

        LayoutInflater.FromContext(Context)?.Inflate(Resource.Layout.DataScanner, this);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        _overlay?.Init(this, FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));
        _overlay?.AddOverlay();

        if (_regionOfInterest is not null)
        {
            EventHandler<ViewAttachedToWindowEventArgs> @event = null!;
            @event = (_, _) =>
            {
                FrameLayout frame = FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));

                _regionOfInterest?.SetConstraints(Convert.ToInt32(Context.FromPixels(frame.Width)), Convert.ToInt32(Context.FromPixels(frame.Height)));
                _dataDetector.RegionOfInterest = _regionOfInterest?.CalculateRegionOfInterest().ToRectPixel(Context);

                _overlay?.AddRegionOfInterest(_regionOfInterest);

                ViewAttachedToWindow -= @event;
            };

            ViewAttachedToWindow += @event;
        }
    }

    /// <summary>
    /// Handles the data detector's Detected event and forwards it to subscribers.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The list of recognized items.</param>
    private void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        Detected?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the data detector's Cleared event and forwards it to subscribers.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnCleared(object? sender, EventArgs e)
    {
        Cleared?.Invoke(this, EventArgs.Empty);
    }
}
