using AVFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Extensions;
using Plugin.Scanner.Views.iOS;
using System.Runtime.Versioning;
using Plugin.Scanner.iOS;

namespace Plugin.Scanner.Overlays;

/// <summary>
/// Provides iOS-specific base scanner overlay implementation with common UI elements and event handling.
/// </summary>
internal abstract partial class ScannerOverlay
{
    private const int TopBarButtonTopAnchorAdd = 25;
    private const int TopBarButtonTrailingAnchorAdd = 20;
    private const int TopBarButtonHeightAnchor = 50;
    private const int TopBarButtonWidthAnchor = 50;

    private readonly DataScannerBarOverlay _topBar = [];
    private readonly DataScannerBarOverlay _bottomBar = [];
    private readonly UIButton _cancelButton = new(UIButtonType.Close);
    private readonly RecognizedItemButton _barcodeItemButton = [];

    private DataScannerViewController? _dataScannerViewController;
    private DataScannerRegionOfInterest? _dataScannerRegionOfInterest;
    private DataScannerTorchButton? _torchButton;

    /// <summary>
    /// Releases resources used by the overlay.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Adds the overlay UI elements including top and bottom bars, cancel button, and torch button to the scanner view.
    /// </summary>
    public void AddOverlay()
    {
        AddOverlayView();
        AddCancelButton();
        AddBarcodeButton();

        if (OperatingSystem.IsIOSVersionAtLeast(17))
        {
            AddTorchButton();
        }
    }

    /// <summary>
    /// Adds a region of interest overlay to restrict scanning to a specific area.
    /// </summary>
    /// <param name="regionOfInterest">The region of interest, or <c>null</c> to scan the entire view.</param>
    public void AddRegionOfInterest(IRegionOfInterest? regionOfInterest)
    {
        if (regionOfInterest is null)
        {
            return;
        }

        _ = _dataScannerViewController?.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _dataScannerRegionOfInterest = new(regionOfInterest);

        _dataScannerViewController.View.Add(_dataScannerRegionOfInterest);

        _dataScannerRegionOfInterest.SetupStroke();
        _dataScannerRegionOfInterest.StartStrokeAnimation();
    }

    /// <summary>
    /// Cleans up overlay resources, removes event handlers, and detaches UI elements.
    /// </summary>
    public void Cleanup()
    {
        _dataScannerViewController?.Added -= OnAdded;
        _dataScannerViewController?.Removed -= OnRemoved;

        if (_dataScannerViewController?.RecognizesMultipleItems == true)
        {
            _dataScannerViewController?.Tapped -= OnTapped;
        }

        _cancelButton.RemoveFromSuperview();

        _torchButton?.RemoveFromSuperview();

        _topBar.RemoveFromSuperview();

        _bottomBar.RemoveFromSuperview();

        _dataScannerRegionOfInterest?.StopStrokeAnimation();
        _dataScannerRegionOfInterest?.RemoveFromSuperview();

        _barcodeItemButton.RemoveFromSuperview();
    }

    /// <summary>
    /// Initializes the overlay with the specified view controller and subscribes to scanner events.
    /// </summary>
    /// <param name="viewController">The view controller containing the overlay.</param>
    public void Init(UIViewController viewController)
    {
        if (viewController is DataScannerViewController barcodeScannerViewController)
        {
            _dataScannerViewController = barcodeScannerViewController;

            _dataScannerViewController.Added += OnAdded;
            _dataScannerViewController.Removed += OnRemoved;

            if (_dataScannerViewController.RecognizesMultipleItems)
            {
                _dataScannerViewController.Tapped += OnTapped;
            }
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the overlay and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources
            _cancelButton.Dispose();
            _torchButton?.Dispose();
            _topBar.Dispose();
            _bottomBar.Dispose();
            _dataScannerRegionOfInterest?.Dispose();
            _barcodeItemButton.Dispose();
        }
    }

    /// <summary>
    /// Handles the torch button toggle event and sets the camera torch mode.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The torch mode to set.</param>
    [SupportedOSPlatform("ios17.0")]
    private static void TorchButtonToggled(object? sender, AVCaptureTorchMode e)
    {
        DataScannerViewController.SetTorchMode(e);
    }

    /// <summary>
    /// Adds the top and bottom bar overlays to the scanner container view.
    /// </summary>
    private void AddOverlayView()
    {
        _ = _dataScannerViewController?.OverlayContainerView ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _dataScannerViewController.OverlayContainerView.AddSubviews(_topBar, _bottomBar);

        NSLayoutConstraint.ActivateConstraints(
        [
            _topBar.TopAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.TopAnchor),
            _topBar.LeadingAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.LeadingAnchor),
            _topBar.TrailingAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.TrailingAnchor),
            _topBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height),

            _bottomBar.BottomAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.BottomAnchor),
            _bottomBar.LeadingAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.LeadingAnchor),
            _bottomBar.TrailingAnchor.ConstraintEqualTo(_dataScannerViewController.OverlayContainerView.LeadingAnchor),
            _bottomBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height)
        ]);
    }

    /// <summary>
    /// Adds the cancel button to the top-right corner of the scanner view.
    /// </summary>
    private void AddCancelButton()
    {
        const int buttonCornerRadius = 100;

        _ = _dataScannerViewController?.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;

        UIButtonConfiguration config = UIButtonConfiguration.FilledButtonConfiguration;
        config.BaseBackgroundColor = UIColor.White;
        config.BaseForegroundColor = UIColor.Black;
        config.Background.CornerRadius = buttonCornerRadius;
        _cancelButton.Configuration = config;

        EventHandler @event = null!;
        @event = (s, _) =>
        {
            ((UIButton)s!).TouchUpInside -= @event;

            _dataScannerViewController.DismissViewController(string.Empty);
        };
        _cancelButton.TouchUpInside += @event;

        _dataScannerViewController.View.Add(_cancelButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _cancelButton.TopAnchor.ConstraintEqualTo(_dataScannerViewController.View.TopAnchor, constant: TopBarButtonTopAnchorAdd),
            _cancelButton.TrailingAnchor.ConstraintEqualTo(_dataScannerViewController.View.TrailingAnchor, constant: -TopBarButtonTrailingAnchorAdd),
            _cancelButton.HeightAnchor.ConstraintEqualTo(TopBarButtonHeightAnchor),
            _cancelButton.WidthAnchor.ConstraintEqualTo(TopBarButtonWidthAnchor),
        ]);
    }

    /// <summary>
    /// Adds the recognized item button to the bottom center of the scanner view.
    /// </summary>
    private void AddBarcodeButton()
    {
        const float buttonWidthAnchorAdd = 30f;
        const float buttonBottomAnchorAdd = 25f;

        _ = _dataScannerViewController?.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        EventHandler @event = null!;
        @event = (s, _) =>
        {
            ((RecognizedItemButton)s!).TouchUpInside -= @event;

            _dataScannerViewController.DismissViewController(((RecognizedItemButton)s).Barcode?.Text ?? string.Empty);
        };
        _barcodeItemButton.TouchUpInside += @event;

        _dataScannerViewController.View.Add(_barcodeItemButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _barcodeItemButton.CenterXAnchor.ConstraintEqualTo(_dataScannerViewController.View.CenterXAnchor),
            _barcodeItemButton.BottomAnchor.ConstraintEqualTo(_dataScannerViewController.View.BottomAnchor, -(DataScannerBarOverlay.Height + buttonBottomAnchorAdd)),
            _barcodeItemButton.WidthAnchor.ConstraintLessThanOrEqualTo(_dataScannerViewController.View.WidthAnchor, constant: -buttonWidthAnchorAdd),
        ]);
    }

    /// <summary>
    /// Adds the torch (flashlight) button to the top-left corner of the scanner view.
    /// </summary>
    [SupportedOSPlatform("ios17.0")]
    private void AddTorchButton()
    {
        _ = _dataScannerViewController?.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _torchButton = new DataScannerTorchButton();
        _torchButton.Toggled += TorchButtonToggled;

        _dataScannerViewController.View.Add(_torchButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _torchButton.TopAnchor.ConstraintEqualTo(_dataScannerViewController.View.TopAnchor, constant: TopBarButtonTopAnchorAdd),
            _torchButton.LeadingAnchor.ConstraintEqualTo(_dataScannerViewController.View.LeadingAnchor, constant: TopBarButtonTrailingAnchorAdd),
            _torchButton.HeightAnchor.ConstraintEqualTo(TopBarButtonHeightAnchor),
            _torchButton.WidthAnchor.ConstraintEqualTo(TopBarButtonWidthAnchor),
        ]);
    }

    /// <summary>
    /// Handles the added event when new items are recognized and displays the first item.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The added items and all items tuple.</param>
    private void OnAdded(object? sender, (iOS.Binding.RecognizedItem[] AddedItems, iOS.Binding.RecognizedItem[] AllItems) e)
    {
        if (_dataScannerViewController?.RecognizesMultipleItems == false)
        {
            _barcodeItemButton.Barcode = e.AddedItems[0].ToRecognizedItem();
            _barcodeItemButton.Hidden = false;
        }
    }

    /// <summary>
    /// Handles the removed event when items are no longer detected and hides the button if the current item was removed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The removed items and all items tuple.</param>
    private void OnRemoved(object? sender, (iOS.Binding.RecognizedItem[] RemovedItems, iOS.Binding.RecognizedItem[] AllItems) e)
    {
        if (e.RemovedItems.Any(x => x.ToRecognizedItem().Id.Equals(_barcodeItemButton.Barcode?.Id, StringComparison.Ordinal)))
        {
            _barcodeItemButton.Hidden = true;
            _barcodeItemButton.Barcode = null;
        }
    }

    /// <summary>
    /// Handles the tapped event when an item is tapped and displays the selected item.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The tapped recognized item.</param>
    private void OnTapped(object? sender, iOS.Binding.RecognizedItem e)
    {
        _barcodeItemButton.Barcode = e.ToRecognizedItem();
        _barcodeItemButton.Hidden = false;
    }
}