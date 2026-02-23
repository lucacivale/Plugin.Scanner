using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Barcode;

/// <summary>
/// A view controller for scanning barcodes using the device camera with a customizable UI overlay.
/// </summary>
internal sealed class BarcodeScannerViewController : DataScannerViewController
{
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest _regionOfInterest;

    private TaskCompletionSource<string>? _scanCompleteTaskSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScannerViewController"/> class.
    /// </summary>
    /// <param name="recognizedDataTypes">An array of <see cref="RecognizedDataType"/> specifying the types of data to recognize during scanning.</param>
    /// <param name="qualityLevel">The <see cref="QualityLevel"/> for scanning. Default is <see cref="QualityLevel.Balanced"/>.</param>
    /// <param name="recognizesMultipleItems">If <c>true</c>, the scanner can recognize multiple items simultaneously. Default is <c>false</c>.</param>
    /// <param name="isHighFrameRateTrackingEnabled">If <c>true</c>, enables high frame rate tracking for better performance. Default is <c>true</c>.</param>
    /// <param name="isPinchToZoomEnabled">If <c>true</c>, allows pinch-to-zoom gesture. Default is <c>true</c>.</param>
    /// <param name="isGuidanceEnabled">If <c>true</c>, displays guidance to the user. Default is <c>true</c>.</param>
    /// <param name="isHighlightingEnabled">If <c>true</c>, highlights recognized items. Default is <c>true</c>.</param>
    public BarcodeScannerViewController(
        RecognizedDataType[] recognizedDataTypes,
        QualityLevel qualityLevel = QualityLevel.Balanced,
        bool recognizesMultipleItems = false,
        bool isHighFrameRateTrackingEnabled = true,
        bool isPinchToZoomEnabled = true,
        bool isGuidanceEnabled = true,
        bool isHighlightingEnabled = true,
        IRegionOfInterest? regionOfInterest = null,
        IOverlay? overlay = null)
        : base(
            recognizedDataTypes,
            qualityLevel,
            recognizesMultipleItems,
            isHighFrameRateTrackingEnabled,
            isPinchToZoomEnabled,
            isGuidanceEnabled,
            isHighlightingEnabled)
    {
        _overlay = overlay;
        _overlay?.Init(this);
    }

    public void DismissViewController(string result)
    {
        StopScanning();
        DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(string.Empty));
    }

    public async Task<IBarcode> ScanAsync(CancellationToken cancellationToken)
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new BarcodeScanException("Failed to find top UIViewController.");

        StartScanning();

        _scanCompleteTaskSource = new();

        BecameUnavailable += OnBecameUnavailable;

        await topViewController.PresentViewControllerAsync(this, true).ConfigureAwait(true);

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        BecameUnavailable -= OnBecameUnavailable;

        _overlay?.Cleanup();

        return new Core.Barcode.Barcode(barcode);
    }

    /// <summary>
    /// Sets up the overlay bars, cancel button, and torch button (iOS 17+).
    /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        ModalInPresentation = true;

        _overlay?.AddOverlay();
        _overlay?.AddRegionOfInterest(_regionOfInterest);
    }

    /// <summary>
    /// Animates the UI elements to fade in.
    /// </summary>
    /// <param name="animated">If <c>true</c>, the appearance is animated.</param>
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        if (View is not null
            && _regionOfInterest is not null)
        {
            _regionOfInterest.SetConstraints((int)View.Frame.Width.Value, (int)View.Frame.Height.Value);

            RegionOfInterest = _regionOfInterest.CalculateRegionOfInterest().ToRect();
        }
    }

    public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
    {
        base.ViewWillTransitionToSize(toSize, coordinator);

        coordinator.AnimateAlongsideTransition(
            _ =>
            {
                if (View is not null
                    && _regionOfInterest is not null)
                {
                    _regionOfInterest.SetConstraints((int)View.Frame.Width.Value, (int)View.Frame.Height.Value);
                    RegionOfInterest = _regionOfInterest.CalculateRegionOfInterest().ToRect();
                }
            },
            null);
    }

    private void OnBecameUnavailable(object? sender, DataScannerUnavailableException e)
    {
        StopScanning();
        DismissViewController(true, () => throw e);
    }
}