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
/// Provides a managed wrapper around the native data scanner view controller with enhanced event handling and error management.
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
    /// <param name="recognizedDataTypes">An array of <see cref="RecognizedDataType"/> specifying the types of data to recognize during scanning.</param>
    /// <param name="qualityLevel">The <see cref="QualityLevel"/> for scanning. Default is <see cref="QualityLevel.Balanced"/>.</param>
    /// <param name="recognizesMultipleItems">If <c>true</c>, the scanner can recognize multiple items simultaneously. Default is <c>false</c>.</param>
    /// <param name="isHighFrameRateTrackingEnabled">If <c>true</c>, enables high frame rate tracking for better performance. Default is <c>true</c>.</param>
    /// <param name="isPinchToZoomEnabled">If <c>true</c>, allows pinch-to-zoom gesture. Default is <c>true</c>.</param>
    /// <param name="isGuidanceEnabled">If <c>true</c>, displays guidance to the user. Default is <c>true</c>.</param>
    /// <param name="isHighlightingEnabled">If <c>true</c>, highlights recognized items. Default is <c>true</c>.</param>
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
    /// Gets or sets the event handler invoked when the scanner zoom level changes.
    /// </summary>
    public EventHandler? Zoomed { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when a recognized item is tapped.
    /// </summary>
    public EventHandler<RecognizedItem>? Tapped { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when items are added to the scanner's recognition results.
    /// </summary>
    public EventHandler<(RecognizedItem[] AddedItems, RecognizedItem[] AllItems)>? Added { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when recognized items are updated.
    /// </summary>
    public EventHandler<(RecognizedItem[] UpdatedItems, RecognizedItem[] AllItems)>? Updated { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when items are removed from the scanner's recognition results.
    /// </summary>
    public EventHandler<(RecognizedItem[] RemovedItems, RecognizedItem[] AllItems)>? Removed { get; set; }

    /// <summary>
    /// Sets the device's torch (flashlight) mode.
    /// Available only on iOS 17.0 and later.
    /// </summary>
    /// <param name="mode">The <see cref="AVCaptureTorchMode"/> to set.</param>
    /// <exception cref="DataScannerTorchUnavailableException">Thrown when the torch is not available on the device.</exception>
    /// <exception cref="DataScannerTorchModeUnsupportedException">Thrown when the specified torch mode is not supported on the device.</exception>
    /// <exception cref="DataScannerCameraConfigurationLockException">Thrown when the camera configuration cannot be locked for modification.</exception>
    /// <remarks>
    /// This method uses the user's preferred camera device to control the torch.
    /// </remarks>
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
    /// Starts the scanning process and subscribes to scanner events.
    /// </summary>
    /// <exception cref="DataScannerUnsupportedException">Thrown when the data scanner is not supported on the device.</exception>
    /// <exception cref="DataScannerUnavailableException">Thrown when the data scanner is not currently available.</exception>
    /// <exception cref="DataScannerStartException">Thrown when the scanner fails to start.</exception>
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
    /// Stops the scanning process and unsubscribes from scanner events.
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

    public void DismissViewController(string result)
    {
        StopScanning();
        DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(result));
    }

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
    /// Sets up the overlay bars, cancel button, and torch button (iOS 17+).
    /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        ModalInPresentation = true;

        _overlay?.AddOverlay();
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

            _overlay?.AddRegionOfInterest(_regionOfInterest);
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

                    foreach (UIView viewSubview in View.Subviews)
                    {
                        viewSubview.LayoutSubviews();
                    }
                }
            },
            null);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the view controller and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
    /// Handles the delegate zoom event and forwards it to the public <see cref="Zoomed"/> event.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments.</param>
    private void DataScannerDidZoom(object? sender, EventArgs e)
    {
        Zoomed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the delegate tap event and forwards it to the public <see cref="Tapped"/> event.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="item">The <see cref="RecognizedItem"/> that was tapped.</param>
    private void DidTapOn(object? sender, RecognizedItem item)
    {
        Tapped?.Invoke(this, item);
    }

    /// <summary>
    /// Handles the delegate item added event and forwards it to the public <see cref="Added"/> event.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">A tuple containing the added items and all currently recognized items.</param>
    private void DidAdd(object? sender, (RecognizedItem[] AddedItems, RecognizedItem[] AllItems) args)
    {
        Added?.Invoke(this, args);
    }

    /// <summary>
    /// Handles the delegate item updated event and forwards it to the public <see cref="Updated"/> event.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">A tuple containing the updated items and all currently recognized items.</param>
    private void DidUpdate(object? sender, (RecognizedItem[] UddedItems, RecognizedItem[] AllItems) args)
    {
        Updated?.Invoke(this, args);
    }

    /// <summary>
    /// Handles the delegate item removed event and forwards it to the public <see cref="Removed"/> event.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="args">A tuple containing the removed items and all currently recognized items.</param>
    private void DidRemove(object? sender, (RecognizedItem[] RddedItems, RecognizedItem[] AllItems) args)
    {
        Removed?.Invoke(this, args);
    }

    private void BecameUnavailableWithError(object? sender, DataScannerUnavailableException error)
    {
        StopScanning();
        DismissViewController(true, () => throw error);
    }
}