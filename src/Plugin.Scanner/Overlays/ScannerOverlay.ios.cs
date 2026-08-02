using System.Diagnostics.CodeAnalysis;
using AVFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.Views.iOS;
using System.Runtime.Versioning;
using Plugin.Scanner.Core.Controllers;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.iOS;

namespace Plugin.Scanner.Overlays;

/// <summary>
/// Provides iOS-specific base scanner overlay implementation with common UI elements and event handling.
/// </summary>
internal abstract partial class ScannerOverlay
{
    private const int DialogMargin = 25;
    private const float PopupMargin = 12.5f;
    private const int TopButtonHeightAnchor = 50;
    private const int TopButtonWidthAnchor = 50;

    private readonly DataScannerBarOverlay _topBar = [];
    private readonly DataScannerBarOverlay _bottomBar = [];
    private readonly IconButton _cancelButton = new("x.circle.fill");
    private readonly IconButton _openResultsButton = new("dot.viewfinder");
    private readonly RecognizedItemButton _barcodeItemButton = [];
    private readonly DataScannerResultsViewController _resultsViewController;
    private readonly UINavigationController _resultsViewNavigationController;

    private UIView? _root;
    private IDataScannerController? _controller;

    private float _margin;

    private DataScannerRegionOfInterest? _dataScannerRegionOfInterest;
    private DataScannerTorchButton? _torchButton;
    private DataScannerExpandMinimizeButton? _expandMinimizeButton;

    protected ScannerOverlay()
    {
        _resultsViewController = new();
        _resultsViewNavigationController = new(_resultsViewController);
    }

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
        if (_controller?.IsDialog == true)
        {
            AddOverlayView();
        }

        AddCancelButton();
        AddBarcodeButton();

        if (OperatingSystem.IsIOSVersionAtLeast(17))
        {
            AddTorchButton();
        }

        if (_controller?.IsDialog == false)
        {
            AddExpandMinimizeButton();
        }

        AddOpenResultsButton();
    }

    /// <summary>
    /// Adds a region of interest overlay to restrict scanning to a specific area.
    /// </summary>
    /// <param name="regionOfInterest">The region of interest, or <c>null</c> to scan the entire view.</param>
    public void AddRegionOfInterest(IRegionOfInterest? regionOfInterest)
    {
        if (regionOfInterest is null
            || _root is null)
        {
            return;
        }

        _dataScannerRegionOfInterest = new(regionOfInterest);
        _dataScannerRegionOfInterest.TranslatesAutoresizingMaskIntoConstraints = false;

        _root.Add(_dataScannerRegionOfInterest);

        NSLayoutConstraint.ActivateConstraints(
        [
            _dataScannerRegionOfInterest.TopAnchor.ConstraintEqualTo(_root.TopAnchor),
            _dataScannerRegionOfInterest.LeadingAnchor.ConstraintEqualTo(_root.LeadingAnchor),
            _dataScannerRegionOfInterest.TrailingAnchor.ConstraintEqualTo(_root.TrailingAnchor),
            _dataScannerRegionOfInterest.BottomAnchor.ConstraintEqualTo(_root.BottomAnchor),
        ]);

        _dataScannerRegionOfInterest.SetupStroke();
        _dataScannerRegionOfInterest.StartStrokeAnimation();
    }

    /// <summary>
    /// Cleans up overlay resources, removes event handlers, and detaches UI elements.
    /// </summary>
    public void Cleanup()
    {
        _resultsViewNavigationController.DismissViewController(true, null);

        _controller?.Added -= OnAdded;
        _controller?.Removed -= OnRemoved;

        if (_controller?.RecognizeMultiple == true)
        {
            _controller?.Tapped -= OnTapped;
        }

        _barcodeItemButton.RemoveFromSuperview();
        _barcodeItemButton.TouchUpInside -= BarcodeItemButtonOnTouchUpInside;

        _cancelButton.RemoveFromSuperview();

        _openResultsButton.TouchUpInside -= OpenResultsClicked;
        _openResultsButton.RemoveFromSuperview();

        _torchButton?.Toggled -= TorchButtonToggled;
        _torchButton?.RemoveFromSuperview();

        _expandMinimizeButton?.Toggled -= ExpandMinimizeButtonToggled;
        _expandMinimizeButton?.RemoveFromSuperview();

        _topBar.RemoveFromSuperview();

        _bottomBar.RemoveFromSuperview();

        _dataScannerRegionOfInterest?.StopStrokeAnimation();
        _dataScannerRegionOfInterest?.RemoveFromSuperview();

        _barcodeItemButton.RemoveFromSuperview();
    }

    /// <summary>
    /// Initializes the overlay with the specified view controller and subscribes to scanner events.
    /// </summary>
    /// <param name="controller">The data scanner controller.</param>
    /// <param name="root">The root view to attach the overlay to.</param>
    public void Init(IDataScannerController controller, UIView root)
    {
        _root = root;
        _controller = controller;

        _controller.Added += OnAdded;
        _controller.Removed += OnRemoved;

        if (_controller.RecognizeMultiple)
        {
            _controller.Tapped += OnTapped;
        }

        _margin = _controller.IsDialog ? DialogMargin : PopupMargin;
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
            _openResultsButton.Dispose();
            _torchButton?.Dispose();
            _expandMinimizeButton?.Dispose();
            _topBar.Dispose();
            _bottomBar.Dispose();
            _dataScannerRegionOfInterest?.Dispose();
            _barcodeItemButton.Dispose();

            _resultsViewController.Dispose();
            _resultsViewNavigationController.Dispose();
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

    private void ExpandMinimizeButtonToggled(object? sender, bool e)
    {
        if (e)
        {
            _controller?.Expand();
        }
        else
        {
            _controller?.Minimize();
        }
    }

    /// <summary>
    /// Adds the top and bottom bar overlays to the scanner container view.
    /// </summary>
    private void AddOverlayView()
    {
        _ = _controller?.Overlay ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _controller.Overlay.AddSubviews(_topBar, _bottomBar);

        NSLayoutConstraint.ActivateConstraints(
        [
            _topBar.TopAnchor.ConstraintEqualTo(_controller.Overlay.TopAnchor),
            _topBar.LeadingAnchor.ConstraintEqualTo(_controller.Overlay.LeadingAnchor),
            _topBar.TrailingAnchor.ConstraintEqualTo(_controller.Overlay.TrailingAnchor),
            _topBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height),

            _bottomBar.BottomAnchor.ConstraintEqualTo(_controller.Overlay.BottomAnchor),
            _bottomBar.LeadingAnchor.ConstraintEqualTo(_controller.Overlay.LeadingAnchor),
            _bottomBar.TrailingAnchor.ConstraintEqualTo(_controller.Overlay.TrailingAnchor),
            _bottomBar.HeightAnchor.ConstraintEqualTo(DataScannerBarOverlay.Height),
        ]);
    }

    /// <summary>
    /// Adds the cancel button to the top-right corner of the scanner view.
    /// </summary>
    private void AddCancelButton()
    {
        _ = _root ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        EventHandler @event = null!;
        @event = (s, _) =>
        {
            ((UIButton)s!).TouchUpInside -= @event;

            _controller?.Dismiss(string.Empty);
        };
        _cancelButton.TouchUpInside += @event;

        _root.Add(_cancelButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _cancelButton.TopAnchor.ConstraintEqualTo(_root.TopAnchor, constant: _margin),
            _cancelButton.TrailingAnchor.ConstraintEqualTo(_root.TrailingAnchor, constant: -_margin),
            _cancelButton.HeightAnchor.ConstraintEqualTo(TopButtonHeightAnchor),
            _cancelButton.WidthAnchor.ConstraintEqualTo(TopButtonWidthAnchor),
        ]);
    }

    /// <summary>
    /// Adds the recognized item button to the bottom center of the scanner view.
    /// </summary>
    private void AddBarcodeButton()
    {
        const float buttonWidthAnchorAdd = 30f;
        const float buttonBottomAnchorAdd = 25f;

        _ = _root ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _barcodeItemButton.TouchUpInside += BarcodeItemButtonOnTouchUpInside;

        _root.Add(_barcodeItemButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _barcodeItemButton.CenterXAnchor.ConstraintEqualTo(_root.CenterXAnchor),
            _barcodeItemButton.BottomAnchor.ConstraintEqualTo(_root.BottomAnchor, -(DataScannerBarOverlay.Height + buttonBottomAnchorAdd)),
            _barcodeItemButton.WidthAnchor.ConstraintLessThanOrEqualTo(_root.WidthAnchor, constant: -buttonWidthAnchorAdd),
        ]);
    }

    private void BarcodeItemButtonOnTouchUpInside(object? sender, EventArgs e)
    {
        if (_controller?.IsDialog == true)
        {
            _controller?.Dismiss(_barcodeItemButton.Barcode?.Text ?? string.Empty);
        }
        else if (_barcodeItemButton.Barcode is not null)
        {
            _resultsViewController.Add(_barcodeItemButton.Barcode);
        }
    }

    /// <summary>
    /// Adds the torch (flashlight) button to the top-left corner of the scanner view.
    /// </summary>
    [SupportedOSPlatform("ios17.0")]
    private void AddTorchButton()
    {
        _ = _root ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _torchButton = new DataScannerTorchButton();
        _torchButton.Toggled += TorchButtonToggled;

        _root.Add(_torchButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _torchButton.TopAnchor.ConstraintEqualTo(_root.TopAnchor, constant: _margin),
            _torchButton.LeadingAnchor.ConstraintEqualTo(_root.LeadingAnchor, constant: _margin),
            _torchButton.HeightAnchor.ConstraintEqualTo(TopButtonHeightAnchor),
            _torchButton.WidthAnchor.ConstraintEqualTo(TopButtonWidthAnchor),
        ]);
    }

    private void AddExpandMinimizeButton()
    {
        _ = _root ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _expandMinimizeButton = new DataScannerExpandMinimizeButton();
        _expandMinimizeButton.Toggled += ExpandMinimizeButtonToggled;

        _root.Add(_expandMinimizeButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _expandMinimizeButton.BottomAnchor.ConstraintEqualTo(_root.BottomAnchor, constant: -_margin),
            _expandMinimizeButton.LeadingAnchor.ConstraintEqualTo(_root.LeadingAnchor, constant: _margin),
            _expandMinimizeButton.HeightAnchor.ConstraintEqualTo(TopButtonHeightAnchor),
            _expandMinimizeButton.WidthAnchor.ConstraintEqualTo(TopButtonWidthAnchor),
        ]);
    }

    private void AddOpenResultsButton()
    {
        _ = _root ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        _openResultsButton.TouchUpInside += OpenResultsClicked;

        _root.Add(_openResultsButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            _openResultsButton.BottomAnchor.ConstraintEqualTo(_root.BottomAnchor, constant: -_margin),
            _openResultsButton.TrailingAnchor.ConstraintEqualTo(_root.TrailingAnchor, constant: -_margin),
            _openResultsButton.HeightAnchor.ConstraintEqualTo(TopButtonHeightAnchor),
            _openResultsButton.WidthAnchor.ConstraintEqualTo(TopButtonWidthAnchor),
        ]);
    }

    /// <summary>
    /// Handles the added event when new items are recognized and displays the first item.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The added items and all items tuple.</param>
    [SuppressMessage("Documentation Rules", "S1172:Remove this unused method parameter 'sende", Justification = "Event handler.")]
    private void OnAdded(object? sender, (RecognizedItem[] AddedItems, RecognizedItem[] AllItems) e)
    {
        if (_controller?.RecognizeMultiple == false)
        {
            _barcodeItemButton.Barcode = e.AddedItems[0];
            _barcodeItemButton.Hidden = false;
        }
    }

    /// <summary>
    /// Handles the removed event when items are no longer detected and hides the button if the current item was removed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The removed items and all items tuple.</param>
    [SuppressMessage("Documentation Rules", "S1172:Remove this unused method parameter 'sende", Justification = "Event handler.")]
    private void OnRemoved(object? sender, (RecognizedItem[] RemovedItems, RecognizedItem[] AllItems) e)
    {
        if (e.RemovedItems.Any(x => x.Id.Equals(_barcodeItemButton.Barcode?.Id, StringComparison.Ordinal)))
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
    [SuppressMessage("Documentation Rules", "S1172:Remove this unused method parameter 'sender", Justification = "Event handler.")]
    private void OnTapped(object? sender, RecognizedItem e)
    {
        _barcodeItemButton.Barcode = e;
        _barcodeItemButton.Hidden = false;
    }

    private void OpenResultsClicked(object? sender, EventArgs e)
    {
        if (_resultsViewController.IsOpen)
        {
            return;
        }

        WindowUtils.GetTopViewController()?.PresentViewController(_resultsViewNavigationController, true, null);
    }
}
