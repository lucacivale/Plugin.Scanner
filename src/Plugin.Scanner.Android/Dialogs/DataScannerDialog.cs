using AndroidX.AppCompat.App;
using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Dialogs;

/// <summary>
/// Provides a fullscreen dialog for scanning data using the device camera on Android.
/// </summary>
internal abstract class DataScannerDialog : AppCompatDialog
{
    private readonly Activity _activity;
    private readonly IDataDetector _dataDetector;

    private readonly bool _recognizeMultiple;
    private readonly bool _isHighlightingEnabled;
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest? _regionOfInterest;

    private TaskCompletionSource<RecognizedItem>? _scanCompleteTaskSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerDialog"/> class.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="detector">The data detector to use for recognition.</param>
    /// <param name="cameraController">The camera controller for managing camera operations.</param>
    /// <param name="regionOfInterest">Optional region of interest to limit scanning area.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    /// <param name="recognizeMultiple">Whether to recognize multiple items.</param>
    /// <param name="isHighlightingEnabled">Whether to highlight detected items.</param>
    protected DataScannerDialog(
        Activity context,
        IDataDetector detector,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_DataScannerDialog)
    {
        _activity = context;
        _dataDetector = detector;
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

    /// <summary>
    /// Starts the scanning operation and waits for a recognized item.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous scan operation, containing the recognized item.</returns>
    /// <exception cref="NoCameraException">Thrown when the device has no camera.</exception>
    public virtual async Task<RecognizedItem> ScanAsync(CancellationToken cancellationToken)
    {
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

    /// <summary>
    /// Shows the scanner dialog and checks for required permissions.
    /// </summary>
    public override void Show()
    {
        PermissionsHelper.CheckPermissions(_activity);

        base.Show();
    }

    /// <summary>
    /// Dismisses the scanner dialog with an empty result.
    /// </summary>
    public override void Dismiss()
    {
        Cleanup();

        _scanCompleteTaskSource?.TrySetResult(RecognizedItem.Empty);
    }

    /// <summary>
    /// Dismisses the scanner dialog with the specified recognized item.
    /// </summary>
    /// <param name="item">The recognized item to return as the scan result.</param>
    public void Dismiss(RecognizedItem item)
    {
        Cleanup();

        _scanCompleteTaskSource?.TrySetResult(item);
    }

    /// <summary>
    /// Cleans up scanner resources, detaches event handlers, and removes the overlay.
    /// </summary>
    protected virtual void Cleanup()
    {
        _dataDetector.Stop();
        _dataDetector.Detected -= OnDetected;
        _dataDetector.Cleared -= OnCleared;

        _overlay?.Cleanup();
    }

    /// <summary>
    /// Initializes and configures the scanner dialog's content view with camera preview, overlay, and region of interest.
    /// </summary>
    protected virtual void SetContentView()
    {
        _overlay?.Init(this, FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.cameraScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));
        _overlay?.AddOverlay();

        if (_regionOfInterest is not null)
        {
            EventHandler @event = null!;
            @event = (_, _) =>
            {
                FrameLayout frame = FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.cameraScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));

                _regionOfInterest?.SetConstraints(Convert.ToInt32(Context.FromPixels(frame.Width)), Convert.ToInt32(Context.FromPixels(frame.Height)));
                _dataDetector.RegionOfInterest = _regionOfInterest?.CalculateRegionOfInterest().ToRectPixel(Context);

                _overlay?.AddRegionOfInterest(_regionOfInterest);

                ShowEvent -= @event;
            };

            ShowEvent += @event;
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
