using AndroidX.Camera.Core;
using AndroidX.Camera.View;
using Plugin.Scanner.Android;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Extensions;
using Plugin.Scanner.Views.Android;
using Orientation = Android.Content.Res.Orientation;

namespace Plugin.Scanner.Overlays.Barcode;

public sealed class DefaultBarcodeScannerOverlay : Java.Lang.Object, IOverlay, View.IOnTouchListener
{
    private IReadOnlyList<RecognizedItem>? _recognizedItems;
    private RecognizedItem? _selectedRecognizedItem;

    private Orientation? _orientation;

    private DataScannerDialog? _dialog;
    private View? _root;
    private RegionOfInterest? _regionOfInterestView;
    private IRegionOfInterest? _regionOfInterest;

    public void Init(Dialog dialog, View root)
    {
        _root = root;

        if (dialog is DataScannerDialog dataScannerDialog)
        {
            _dialog = dataScannerDialog;
        }
    }

    public void AddOverlay()
    {
        PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.SetOnTouchListener(this);

        View? view = _dialog?.LayoutInflater.Inflate(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScannerOverlay, _dialog.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));

        RecognizedItemButton recognizedButton = view?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked += RecognizedItemClicked;

        ImageButton closeButton = view.FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click += CloseButtonClicked;
        closeButton.Visibility = _root.Context?.HasFlash() == false ? ViewStates.Gone : ViewStates.Visible;

        FlashButton flashButton = view.FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled += FlashButton_Toggled;
    }

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
            _root?.LayoutChange += DecorView_LayoutChange;
        }
    }

    public void Detected(IReadOnlyList<RecognizedItem> items)
    {
        _recognizedItems = items;
        RecognizedItem? recognizedItem;

        if (_dialog?.RecognizeMultiple == false)
        {
            _selectedRecognizedItem ??= items.First();

            recognizedItem = items.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) ?? items.First();
        }
        else
        {
            recognizedItem = items.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) ?? null;
        }

        RecognizedItemButton itemButton = _root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = recognizedItem;
        itemButton.Visibility = recognizedItem is null ? ViewStates.Gone : ViewStates.Visible;

        if (_dialog?.IsHighlightingEnabled == true)
        {
            PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));

            if (_dialog?.RecognizeMultiple == false)
            {
                if (recognizedItem is not null)
                {
                    previewView.Overlay?.Add(new RecognizedItemHighlight(recognizedItem));
                }
            }
            else
            {
                foreach (RecognizedItem item in items)
                {
                    previewView.Overlay?.Add(new RecognizedItemHighlight(item));
                }
            }
        }
    }

    public void Cleared()
    {
        RecognizedItemButton itemButton = _root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = null;
        itemButton.Visibility = ViewStates.Gone;
    }

    public bool OnTouch(View? v, MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Down
            && _recognizedItems?.FirstOrDefault(x => x.Bounds.ContainsWithTolerance((int)e.GetX(), (int)e.GetY(), 30)) is RecognizedItem item)
        {
            _selectedRecognizedItem = item;
        }

        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _regionOfInterest = null;
            _root = null;
            _dialog = null;
            _regionOfInterestView?.Dispose();
            _regionOfInterestView = null;
        }

        base.Dispose(disposing);
    }

    private void RecognizedItemClicked(object? sender, RecognizedItem e)
    {
        _dialog?.Dismiss(e);
    }

    private void CloseButtonClicked(object? sender, EventArgs e)
    {
        _dialog?.Cancel();
    }

    private void FlashButton_Toggled(object? sender, int e)
    {
        PreviewView previewView = _root?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller?.ImageCaptureFlashMode = e;
        previewView.Controller?.EnableTorch(e != ImageCapture.FlashModeOff);
    }

    private void DecorView_LayoutChange(object? sender, View.LayoutChangeEventArgs e)
    {
        if (_regionOfInterestView is null)
        {
            return;
        }

        if (_dialog?.IsShowing == true
            && _orientation != _root?.Context?.Resources?.Configuration?.Orientation)
        {
            _orientation = _root?.Context?.Resources?.Configuration?.Orientation;

            FrameLayout frame = _root?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));
            _regionOfInterest?.SetConstraints(Convert.ToInt32(_root?.Context?.FromPixels(frame.Width) ?? 0), Convert.ToInt32(_root?.Context?.FromPixels(frame.Height) ?? 0));

            _regionOfInterestView.Reset();
        }
    }
}