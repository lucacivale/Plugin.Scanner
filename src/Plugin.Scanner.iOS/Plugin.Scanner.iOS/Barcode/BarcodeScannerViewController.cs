using AVFoundation;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.iOS.Barcode.Views;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Views;
using System.Runtime.Versioning;

namespace Plugin.Scanner.iOS.Barcode;

/// <summary>
/// A view controller for scanning barcodes using the device camera with a customizable UI overlay.
/// </summary>
internal sealed class BarcodeScannerViewController : DataScannerViewController
{
    private const int TopBarButtonTopAnchorAdd = 25;
    private const int TopBarButtonTrailingAnchorAdd = 20;
    private const int TopBarButtonHeightAnchor = 50;
    private const int TopBarButtonWidthAnchor = 50;

    private readonly UIButton _cancelButton;
    private readonly DataScannerBarOverlay _topBar;
    private readonly DataScannerBarOverlay _bottomBar;

    private TaskCompletionSource<string>? _scanCompleteTaskSource;
    private DataScannerTorchButton? _torchButton;

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
        bool isHighlightingEnabled = true)
        : base(recognizedDataTypes, qualityLevel, recognizesMultipleItems, isHighFrameRateTrackingEnabled, isPinchToZoomEnabled, isGuidanceEnabled, isHighlightingEnabled)
    {
        _cancelButton = new(UIButtonType.Close);

        _topBar = [];
        _bottomBar = [];
    }

    public async Task<IBarcode> ScanAsync(CancellationToken cancellationToken)
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new BarcodeScanException("Failed to find top UIViewController.");

        StartScanning();

        _scanCompleteTaskSource = new();

        Added += OnAdded;
        Removed += OnRemoved;
        BecameUnavailable += OnBecameUnavailable;

        if (RecognizesMultipleItems)
        {
            Tapped += OnTapped;
        }

        await topViewController.PresentViewControllerAsync(this, true).ConfigureAwait(true);

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        Added -= OnAdded;
        Removed -= OnRemoved;
        BecameUnavailable -= OnBecameUnavailable;

        if (RecognizesMultipleItems)
        {
            Tapped -= OnTapped;
        }

        return new Core.Barcode.Barcode(barcode);
    }

    /// <summary>
    /// Sets up the overlay bars, cancel button, and torch button (iOS 17+).
    /// </summary>
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        ModalInPresentation = true;

        AddOverlay();
        AddCancelButton();

        if (OperatingSystem.IsIOSVersionAtLeast(17))
        {
            AddTorchButton();
        }
    }

    /// <summary>
    /// Animates the UI elements to fade in.
    /// </summary>
    /// <param name="animated">If <c>true</c>, the appearance is animated.</param>
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        UIView.Animate(
            duration: 0.10,
            animation: () =>
            {
                _topBar.Alpha = 1;
                _bottomBar.Alpha = 1;
                _cancelButton.Alpha = 1;
                _torchButton?.Alpha = 1;
            });
    }

    /// <summary>
    /// Releases the unmanaged resources used by the view controller and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancelButton.RemoveFromSuperview();
            _cancelButton.Dispose();

            _torchButton?.RemoveFromSuperview();
            _torchButton?.Dispose();

            _topBar.RemoveFromSuperview();
            _topBar.Dispose();

            _bottomBar.RemoveFromSuperview();
            _bottomBar.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Handles the torch button toggle event and sets the torch mode accordingly.
    /// Available only on iOS 17.0 and later.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The desired torch mode.</param>
    [SupportedOSPlatform("ios17.0")]
    private static void TorchButtonToggled(object? sender, AVCaptureTorchMode e)
    {
        SetTorchMode(e);
    }

    /// <summary>
    /// Adds the top and bottom overlay bars to the scanner view.
    /// </summary>
    private void AddOverlay()
    {
        _topBar.Alpha = 0;
        _bottomBar.Alpha = 0;

        OverlayContainerView.AddSubviews(_topBar, _bottomBar);

        NSLayoutConstraint.ActivateConstraints(
        [
            _topBar.TopAnchor.ConstraintEqualTo(OverlayContainerView.TopAnchor),
            _topBar.LeadingAnchor.ConstraintEqualTo(OverlayContainerView.LeadingAnchor),
            _topBar.TrailingAnchor.ConstraintEqualTo(OverlayContainerView.TrailingAnchor),
            _topBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height),

            _bottomBar.BottomAnchor.ConstraintEqualTo(OverlayContainerView.BottomAnchor),
            _bottomBar.LeadingAnchor.ConstraintEqualTo(OverlayContainerView.LeadingAnchor),
            _bottomBar.TrailingAnchor.ConstraintEqualTo(OverlayContainerView.TrailingAnchor),
            _bottomBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height)
        ]);
    }

    /// <summary>
    /// Adds the cancel button to the top-right corner of the view.
    /// </summary>
    /// <exception cref="DataScannerViewNullReferenceException">Thrown when the view is null.</exception>
    private void AddCancelButton()
    {
        const int buttonCornerRadius = 100;

        _ = View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _cancelButton.Alpha = 0;
        _cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;

        UIButtonConfiguration config = UIButtonConfiguration.FilledButtonConfiguration;
        config.BaseBackgroundColor = UIColor.White;
        config.BaseForegroundColor = UIColor.Black;
        config.Background.CornerRadius = buttonCornerRadius;
        _cancelButton.Configuration = config;

        EventHandler @event = null!;
        @event = (s, e) =>
        {
            ((UIButton)s!).TouchUpInside -= @event;

            CleanupBarcodeItems();

            StopScanning();
            DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(string.Empty));
        };
        _cancelButton.TouchUpInside += @event;

        View.Add(_cancelButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _cancelButton.TopAnchor.ConstraintEqualTo(View.TopAnchor, constant: TopBarButtonTopAnchorAdd),
            _cancelButton.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, constant: -TopBarButtonTrailingAnchorAdd),
            _cancelButton.HeightAnchor.ConstraintEqualTo(TopBarButtonHeightAnchor),
            _cancelButton.WidthAnchor.ConstraintEqualTo(TopBarButtonWidthAnchor),
        ]);
    }

    /// <summary>
    /// Adds the torch (flashlight) button to the top-left corner of the view.
    /// Available only on iOS 17.0 and later.
    /// </summary>
    /// <exception cref="DataScannerViewNullReferenceException">Thrown when the view is null.</exception>
    [SupportedOSPlatform("ios17.0")]
    private void AddTorchButton()
    {
        _ = View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _torchButton = new DataScannerTorchButton();
        _torchButton.Alpha = 0;
        _torchButton.Toggled += TorchButtonToggled;

        View.Add(_torchButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _torchButton.TopAnchor.ConstraintEqualTo(View.TopAnchor, constant: TopBarButtonTopAnchorAdd),
            _torchButton.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, constant: TopBarButtonTrailingAnchorAdd),
            _torchButton.HeightAnchor.ConstraintEqualTo(TopBarButtonHeightAnchor),
            _torchButton.WidthAnchor.ConstraintEqualTo(TopBarButtonWidthAnchor),
        ]);
    }

    private void OnAdded(object? sender, (RecognizedItem[] AddedItems, RecognizedItem[] AllItems) e)
    {
        if (RecognizesMultipleItems == false)
        {
            AddBarcodeItem(e.AddedItems[0]);
        }
    }

    /// <summary>
    /// Handles the event when a barcode item is removed from the scanner view.
    /// Cleans up the associated button UI element.
    /// </summary>
    /// <param name="sender">The <see cref="DataScannerViewController"/> that raised the event.</param>
    /// <param name="e">A tuple containing the removed items and all currently recognized items.</param>
    /// <exception cref="DataScannerViewNullReferenceException">Thrown when the scanner's view is null.</exception>
    private void OnRemoved(object? sender, (RecognizedItem[] RemovedItems, RecognizedItem[] AllItems) e)
    {
        BarcodeItemButton? barcodeItem = View?.Subviews
            .OfType<BarcodeItemButton>()
            .FirstOrDefault(x => e.RemovedItems.Any(y => x.Barcode.Id.Equals(y.Id)));

        if (barcodeItem is not null)
        {
            CleanupBarcodeItem(barcodeItem);
        }
    }

    private void OnBarcodeItemTapped(object? sender, EventArgs e)
    {
        if (sender is not BarcodeItemButton barcodeItem)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type BarcodeItemButton.");
        }

        CleanupBarcodeItems();

        StopScanning();
        DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(barcodeItem.Barcode.Value));
    }

    private void OnBecameUnavailable(object? sender, DataScannerUnavailableException e)
    {
        CleanupBarcodeItems();

        StopScanning();
        DismissViewController(true, () => throw e);
    }

    private void OnTapped(object? sender, RecognizedItem e)
    {
        CleanupBarcodeItems();
        AddBarcodeItem(e);
    }

    private void AddBarcodeItem(RecognizedItem barcode)
    {
        const float buttonWidthAnchorAdd = 30f;
        const float buttonBottomAnchorAdd = 25f;

        _ = View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        BarcodeItemButton barcodeItemButton = new(barcode);
        barcodeItemButton.TouchUpInside += OnBarcodeItemTapped;

        View.Add(barcodeItemButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            barcodeItemButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
            barcodeItemButton.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -(DataScannerBarOverlay.Height + buttonBottomAnchorAdd)),
            barcodeItemButton.WidthAnchor.ConstraintLessThanOrEqualTo(View.WidthAnchor, constant: -buttonWidthAnchorAdd),
        ]);
    }

    private void CleanupBarcodeItems()
    {
        IEnumerable<BarcodeItemButton> barcodeItems = View?.Subviews.OfType<BarcodeItemButton>() ?? [];

        foreach (BarcodeItemButton barcodeItem in barcodeItems)
        {
            CleanupBarcodeItem(barcodeItem);
        }
    }

    /// <summary>
    /// Removes and disposes a single barcode item button.
    /// </summary>
    /// <param name="barcodeItemButton">The <see cref="BarcodeItemButton"/> to clean up.</param>
    private void CleanupBarcodeItem(BarcodeItemButton barcodeItemButton)
    {
        barcodeItemButton.TouchUpInside -= OnBarcodeItemTapped;
        barcodeItemButton.RemoveFromSuperview();
        barcodeItemButton.Dispose();
    }
}