using AndroidX.Camera.Core;
using AndroidX.Camera.View;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Controllers;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Views.Android;
using Orientation = Android.Content.Res.Orientation;

namespace Plugin.Scanner.Overlays;

/// <summary>
/// Provides Android-specific base scanner overlay implementation with common UI elements and event handling.
/// </summary>
internal abstract partial class ScannerOverlay : Java.Lang.Object, View.IOnTouchListener
{
    private IReadOnlyList<RecognizedItem>? _recognizedItems;

    private Orientation? _orientation;

    private IDataScannerController? _controller;
    private View? _root;
    private RegionOfInterest? _regionOfInterestView;
    private IRegionOfInterest? _regionOfInterest;

    /// <summary>
    /// Gets the root view of the scanner.
    /// </summary>
    protected View? Root => _root;

    /// <summary>
    /// Gets the list of currently recognized items.
    /// </summary>
    protected IReadOnlyList<RecognizedItem>? RecognizedItems => _recognizedItems;

    protected IDataScannerController? Controller => _controller;

    /// <summary>
    /// Initializes the overlay with the specified dialog and root view.
    /// </summary>
    /// <param name="controller">The data scanner controller.</param>
    /// <param name="root">The root view to attach the overlay to.</param>
    public void Init(IDataScannerController controller, View root)
    {
        _root = root;

        _controller = controller;
        _controller.Detected += OnDetected;
        _controller.Cleared += OnCleared;
    }

    /// <summary>
    /// Adds the overlay UI elements including buttons and touch listeners to the scanner view.
    /// </summary>
    public void AddOverlay()
    {
        PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.SetOnTouchListener(this);

        int overlayId = Controller?.IsDialog == true ? _Microsoft.Android.Resource.Designer.Resource.Layout.DataScannerDialogOverlay : _Microsoft.Android.Resource.Designer.Resource.Layout.DataScannerPopupOverlay;
        View? view = LayoutInflater.From(_root.Context)?.Inflate(overlayId, _controller?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));

        RecognizedItemButton recognizedButton = view?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked += RecognizedItemClicked;

        ImageButton closeButton = view.FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click += CloseButtonClicked;
        closeButton.Visibility = _root.Context?.HasFlash() == false ? ViewStates.Gone : ViewStates.Visible;

        FlashButton flashButton = view.FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled += FlashButton_Toggled;

        if (view.FindViewById<ExpandMinimizeButton>(_Microsoft.Android.Resource.Designer.Resource.Id.expandminimize) is ExpandMinimizeButton expandMinimize)
        {
            expandMinimize.Toggled += ExpandMinimize_Toggled;
        }
    }

    private void ExpandMinimize_Toggled(object? sender, bool e)
    {
        if (e)
        {
            _controller?.Expand();
        }
        else
        {
            _controller?.Minimize();
        }

        _regionOfInterestView?.PostDelayed(
            () =>
            {
                FrameLayout frame = _root?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));
                _regionOfInterest?.SetConstraints(Convert.ToInt32(_root?.Context?.FromPixels(frame.Width) ?? 0), Convert.ToInt32(_root?.Context?.FromPixels(frame.Height) ?? 0));
                _regionOfInterestView?.Reset();
            },
            250);
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

        _regionOfInterest = regionOfInterest;
        _root?.LayoutChange += DecorView_LayoutChange;
        _orientation = _root?.Context?.Resources?.Configuration?.Orientation;

        FrameLayout frame = _root?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));

        _regionOfInterestView = new(frame.Context, regionOfInterest);

        frame.AddView(_regionOfInterestView);

        _regionOfInterestView.StartStrokeAnimation();
    }

    /// <summary>
    /// Cleans up overlay resources, removes event handlers, and detaches UI elements.
    /// </summary>
    public void Cleanup()
    {
        PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.SetOnTouchListener(null);
        previewView.Overlay?.Clear();

        RecognizedItemButton recognizedButton = _root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked -= RecognizedItemClicked;

        ImageButton closeButton = _root.FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = _root.FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled -= FlashButton_Toggled;

        if (_regionOfInterestView is not null
            && _regionOfInterestView.Parent is ViewGroup frame)
        {
            frame.RemoveView(_regionOfInterestView);
            _root?.LayoutChange -= DecorView_LayoutChange;
        }

        _controller?.Detected -= OnDetected;
        _controller?.Cleared -= OnCleared;
    }

    /// <summary>
    /// Handles touch events on the scanner view.
    /// </summary>
    /// <param name="v">The view that was touched.</param>
    /// <param name="e">The motion event containing touch data.</param>
    /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
    public abstract bool OnTouch(View? v, MotionEvent? e);

    /// <summary>
    /// Releases resources used by the overlay.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _regionOfInterest = null;
            _root = null;
            _controller = null;
            _regionOfInterestView?.Dispose();
            _regionOfInterestView = null;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Handles the detection event when items are recognized and clears previous highlights.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The list of recognized items.</param>
    protected virtual void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        PreviewView previewView = _controller?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        _recognizedItems = e;
    }

    /// <summary>
    /// Handles the cleared event when no items are detected and hides the recognition UI.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnCleared(object? sender, EventArgs e)
    {
        PreviewView previewView = _controller?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        RecognizedItemButton itemButton = _root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = null;
        itemButton.Visibility = ViewStates.Gone;

        previewView.Invalidate();
    }

    /// <summary>
    /// Handles the recognized item button click and dismisses the dialog with the selected item.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The recognized item that was clicked.</param>
    private void RecognizedItemClicked(object? sender, RecognizedItem e)
    {
        _controller?.Dismiss(e);
    }

    /// <summary>
    /// Handles the close button click and cancels the scanning operation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CloseButtonClicked(object? sender, EventArgs e)
    {
        _controller?.Cancel();
    }

    /// <summary>
    /// Handles the flash button toggle and enables or disables the camera flash.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The flash mode value.</param>
    private void FlashButton_Toggled(object? sender, int e)
    {
        PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller?.ImageCaptureFlashMode = e;
        previewView.Controller?.EnableTorch(e != ImageCapture.FlashModeOff);
    }

    /// <summary>
    /// Handles layout changes and updates the region of interest when device orientation changes.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The layout change event arguments.</param>
    private void DecorView_LayoutChange(object? sender, View.LayoutChangeEventArgs e)
    {
        if (_regionOfInterestView is null)
        {
            return;
        }

        if (_controller?.IsRunning == true
            && _orientation != _root?.Context?.Resources?.Configuration?.Orientation)
        {
            _orientation = _root?.Context?.Resources?.Configuration?.Orientation;

            FrameLayout frame = _root?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));
            _regionOfInterest?.SetConstraints(Convert.ToInt32(_root?.Context?.FromPixels(frame.Width) ?? 0), Convert.ToInt32(_root?.Context?.FromPixels(frame.Height) ?? 0));

            _regionOfInterestView.Reset();
        }
    }
}
