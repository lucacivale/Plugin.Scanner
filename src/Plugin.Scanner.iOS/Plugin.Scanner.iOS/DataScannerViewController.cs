using System.Runtime.Versioning;
using AVFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Extensions;
using RecognizedItem = Plugin.Scanner.iOS.Binding.RecognizedItem;

namespace Plugin.Scanner.iOS;

/// <summary>
/// iOS data scanner view controller that handles scanning operations for recognized data types.
/// </summary>
internal sealed class DataScannerViewController : Binding.DataScannerViewController
{
    private readonly DataScannerViewControllerDelegate _dataScannerViewControllerDelegate;
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest? _regionOfInterest;

    private TaskCompletionSource<string>? _scanCompleteTaskSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewController"/> class.
    /// </summary>
    /// <param name="recognizedDataTypes">Types of data to recognize during scanning.</param>
    /// <param name="qualityLevel">Quality level for recognition.</param>
    /// <param name="recognizesMultipleItems">Whether to recognize multiple items simultaneously.</param>
    /// <param name="isHighFrameRateTrackingEnabled">Whether high frame rate tracking is enabled.</param>
    /// <param name="isPinchToZoomEnabled">Whether pinch-to-zoom is enabled.</param>
    /// <param name="isGuidanceEnabled">Whether user guidance is enabled.</param>
    /// <param name="isHighlightingEnabled">Whether highlighting of recognized items is enabled.</param>
    /// <param name="regionOfInterest">Optional region of interest for scanning.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    public DataScannerViewController(
        RecognizedDataType[] recognizedDataTypes,
        QualityLevel qualityLevel = QualityLevel.Balanced,
        bool recognizesMultipleItems = false,
        bool isHighFrameRateTrackingEnabled = true,
        bool isPinchToZoomEnabled = true,
        bool isGuidanceEnabled = true,
        bool isHighlightingEnabled = true,
        IRegionOfInterest? regionOfInterest = null,
        IOverlay? overlay = null)
        : base(recognizedDataTypes, qualityLevel, recognizesMultipleItems, isHighFrameRateTrackingEnabled, isPinchToZoomEnabled, isGuidanceEnabled, isHighlightingEnabled)
    {
        _dataScannerViewControllerDelegate = new DataScannerViewControllerDelegate();
        Delegate = _dataScannerViewControllerDelegate;

        _regionOfInterest = regionOfInterest;
        _overlay = overlay;
        _overlay?.Init(this);
    }

    /// <summary>
    /// Gets or sets when the scanner zoom level changes.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public EventHandler? Zoomed { get; set; }

    /// <summary>
    /// Gets or sets when a recognized item is tapped.
    /// </summary>
    public EventHandler<RecognizedItem>? Tapped { get; set; }

    /// <summary>
    /// Gets or sets when new items are recognized and added.
    /// </summary>
    public EventHandler<(RecognizedItem[] AddedItems, RecognizedItem[] AllItems)>? Added { get; set; }

    /// <summary>
    /// Gets or sets when recognized items are updated.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public EventHandler<(RecognizedItem[] UpdatedItems, RecognizedItem[] AllItems)>? Updated { get; set; }

    /// <summary>
    /// Gets or sets when recognized items are removed.
    /// </summary>
    public EventHandler<(RecognizedItem[] RemovedItems, RecognizedItem[] AllItems)>? Removed { get; set; }

    /// <summary>
    /// Sets the torch mode for the device camera.
    /// </summary>
    /// <param name="mode">The torch mode to set.</param>
    /// <exception cref="DataScannerTorchUnavailableException">Thrown when the torch is not available on the device.</exception>
    /// <exception cref="DataScannerTorchModeUnsupportedException">Thrown when the specified torch mode is not supported.</exception>
    /// <exception cref="DataScannerCameraConfigurationLockException">Thrown when camera configuration cannot be locked.</exception>
    [SupportedOSPlatform("ios17.0")]
    public static void SetTorchMode(AVCaptureTorchMode mode)
    {
        AVCaptureDevice? camera = AVCaptureDevice.UserPreferredCamera;

        if (camera is null)
        {
            return;
        }

        if (camera.TorchAvailable == false)
        {
            throw new DataScannerTorchUnavailableException("Torch is unsupported on this device.");
        }

        if (camera.IsTorchModeSupported(mode) == false)
        {
            throw new DataScannerTorchModeUnsupportedException($"{mode} is unsupported on this device.");
        }

        camera.LockForConfiguration(out NSError? error);

        using (error)
        {
            if (error is not null)
            {
                throw new DataScannerCameraConfigurationLockException(error.Description);
            }
        }

        camera.TorchMode = mode;
        camera.UnlockForConfiguration();
    }

    /// <summary>
    /// Starts the scanning session.
    /// </summary>
    /// <exception cref="DataScannerUnsupportedException">Thrown when data scanning is not supported on the device.</exception>
    /// <exception cref="DataScannerUnavailableException">Thrown when data scanning is currently unavailable.</exception>
    /// <exception cref="DataScannerStartException">Thrown when scanning fails to start.</exception>
    public void StartScanning()
    {
        if (!IsSupported)
        {
            throw new DataScannerUnsupportedException(string.Join(", ", ScanningUnavailable));
        }

        if (!IsAvailable)
        {
            throw new DataScannerUnavailableException(string.Join(", ", ScanningUnavailable));
        }

        StartScanning(out NSError? nsError);

        using (nsError)
        {
            if (nsError is not null)
            {
                throw new DataScannerStartException(nsError.Description);
            }
        }

        _dataScannerViewControllerDelegate.Zoomed += Zoomed;
        _dataScannerViewControllerDelegate.Tapped += DidTapOn;
        _dataScannerViewControllerDelegate.Added += DidAdd;
        _dataScannerViewControllerDelegate.Updated += DidUpdate;
        _dataScannerViewControllerDelegate.Removed += DidRemove;
        _dataScannerViewControllerDelegate.BecameUnavailable += BecameUnavailableWithError;
    }

    /// <summary>
    /// Stops the scanning session and unsubscribes from delegate events.
    /// </summary>
    public override void StopScanning()
    {
        _dataScannerViewControllerDelegate.Zoomed -= DataScannerDidZoom;
        _dataScannerViewControllerDelegate.Tapped -= DidTapOn;
        _dataScannerViewControllerDelegate.Added -= DidAdd;
        _dataScannerViewControllerDelegate.Updated -= DidUpdate;
        _dataScannerViewControllerDelegate.Removed -= DidRemove;
        _dataScannerViewControllerDelegate.BecameUnavailable -= BecameUnavailableWithError;

        base.StopScanning();
    }

    /// <summary>
    /// Dismisses the scanner view controller with the specified result.
    /// </summary>
    /// <param name="result">The scan result to return.</param>
    public void DismissViewController(string result)
    {
        StopScanning();
        DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(result));
    }

    /// <summary>
    /// Presents the scanner and asynchronously waits for a scan result.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the scanning operation.</param>
    /// <returns>The scan result.</returns>
    /// <exception cref="ScanException">Thrown when the top view controller cannot be found.</exception>
    public async Task<IScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new ScanException("Failed to find top UIViewController.");

        StartScanning();

        _scanCompleteTaskSource = new();

        await topViewController.PresentViewControllerAsync(this, true).ConfigureAwait(true);

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        _overlay?.Cleanup();

        return new ScanResult(barcode);
    }

    /// <summary>
    /// Called when the view has been loaded. Sets modal presentation and adds overlay.
    /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        ModalInPresentation = true;

        _overlay?.AddOverlay();
    }

    /// <summary>
    /// Called when the view has appeared. Configures region of interest based on view frame.
    /// </summary>
    /// <param name="animated">Whether the appearance is animated.</param>
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        if (View is not null
            && _regionOfInterest is not null)
        {
            _regionOfInterest.SetConstraints((int)View.Frame.Width.Value, (int)View.Frame.Height.Value);

            RegionOfInterest = _regionOfInterest.CalculateRegionOfInterest().ToRect();

            _overlay?.AddRegionOfInterest(_regionOfInterest);
        }
    }

    /// <summary>
    /// Called when the view is transitioning to a new size. Updates region of interest and layout for new dimensions.
    /// </summary>
    /// <param name="toSize">The new size of the view.</param>
    /// <param name="coordinator">The transition coordinator.</param>
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

                    foreach (UIView viewSubview in View.Subviews)
                    {
                        viewSubview.LayoutSubviews();
                    }
                }
            },
            null);
    }

    /// <summary>
    /// Disposes of managed resources including the delegate.
    /// </summary>
    /// <param name="disposing">Whether disposing is in progress.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Delegate = null;
            _dataScannerViewControllerDelegate.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Handles the zoom event from the delegate and raises the <see cref="Zoomed"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void DataScannerDidZoom(object? sender, EventArgs e)
    {
        Zoomed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the tap event from the delegate and raises the <see cref="Tapped"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="item">The tapped recognized item.</param>
    private void DidTapOn(object? sender, RecognizedItem item)
    {
        Tapped?.Invoke(this, item);
    }

    /// <summary>
    /// Handles the add event from the delegate and raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The added items and all items.</param>
    private void DidAdd(object? sender, (RecognizedItem[] AddedItems, RecognizedItem[] AllItems) args)
    {
        Added?.Invoke(this, args);
    }

    /// <summary>
    /// Handles the update event from the delegate and raises the <see cref="Updated"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The updated items and all items.</param>
    private void DidUpdate(object? sender, (RecognizedItem[] UddedItems, RecognizedItem[] AllItems) args)
    {
        Updated?.Invoke(this, args);
    }

    /// <summary>
    /// Handles the remove event from the delegate and raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The removed items and all items.</param>
    private void DidRemove(object? sender, (RecognizedItem[] RddedItems, RecognizedItem[] AllItems) args)
    {
        Removed?.Invoke(this, args);
    }

    /// <summary>
    /// Handles the unavailable error from the delegate, stops scanning and dismisses the view controller.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="error">The exception indicating why scanning became unavailable.</param>
    private void BecameUnavailableWithError(object? sender, DataScannerUnavailableException error)
    {
        StopScanning();
        DismissViewController(true, () => throw error);
    }
}