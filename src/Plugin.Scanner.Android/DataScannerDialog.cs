using AndroidX.AppCompat.App;
using AndroidX.Camera.Core;
using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Models;
using Plugin.Scanner.Android.Views;

namespace Plugin.Scanner.Android;

internal sealed class DataScannerDialog : AppCompatDialog, View.IOnTouchListener
{
    private readonly Activity _activity;
    private readonly LifecycleCameraController _cameraController;
    private readonly IDataDetector _dataDetector;

    private readonly bool _recognizeMultiple;
    private readonly bool _isHighlightingEnabled;
    private readonly List<RecognizedItemHighlight> _recognizedItemHighlights = [];

    private TaskCompletionSource<RecognizedItem>? _scanCompleteTaskSource;
    private RecognizedItem? _selectedRecognizedItem;
    private IReadOnlyList<RecognizedItem>? _recognizedItems;

    private RegionOfInterest? _regionOfInterest;

    public DataScannerDialog(
        Activity context,
        IDataDetector detector,
        LifecycleCameraController cameraController,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_DataScannerDialog)
    {
        _activity = context;
        _dataDetector = detector;
        _cameraController = cameraController;

        _recognizeMultiple = recognizeMultiple;
        _isHighlightingEnabled = isHighlightingEnabled;

        SetContentView();
    }

    public async Task<RecognizedItem> ScanAsync(CancellationToken cancellationToken)
    {
        if (Context.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        Show();

        _dataDetector.Detected += Detected;
        _dataDetector.Cleared += Cleared;

        _scanCompleteTaskSource = new();

        RecognizedItem result = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        // Wait here because MLKit won't stop analyzing until the pipeline is finished.
        // If we dispose the dialog before MLKit finishes, it will throw an exception.
        await Task.Delay(250, cancellationToken).ConfigureAwait(true);

        return result;
    }

    public override void Show()
    {
        PermissionsHelper.CheckPermissions(_activity);

        base.Show();
    }

    public override void Dismiss()
    {
        Cleanup();

        base.Dismiss();

        _scanCompleteTaskSource?.TrySetResult(RecognizedItem.Empty);
    }

    public override void Cancel()
    {
        base.Cancel();

        _scanCompleteTaskSource?.TrySetResult(RecognizedItem.Empty);
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

    private void Cleanup()
    {
        _dataDetector.Stop();
        _dataDetector.Detected -= Detected;
        _dataDetector.Cleared -= Cleared;

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;
        previewView.SetOnTouchListener(null);

        previewView.Overlay?.Clear();

        _recognizedItemHighlights.ForEach(x => x.Dispose());
        _recognizedItemHighlights.Clear();

        RecognizedItemButton recognizedButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked -= RecognizedItemClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled -= FlashButton_Toggled;

        if (_regionOfInterest is not null
            && _regionOfInterest.Parent is ViewGroup frame)
        {
            _regionOfInterest.StopStrokeAnimation();

            frame.RemoveView(_regionOfInterest);

            _regionOfInterest.Dispose();
        }
    }

    private void SetContentView()
    {
        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScanner);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;
        previewView.SetOnTouchListener(this);

        AddOverlay();
        AddRegionOfInterest();
    }

    private void AddOverlay()
    {
        LayoutInflater.Inflate(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScannerOverlay, FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));

        RecognizedItemButton recognizedButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked += RecognizedItemClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click += CloseButtonClicked;
        closeButton.Visibility = Context.HasFlash() == false ? ViewStates.Gone : ViewStates.Visible;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled += FlashButton_Toggled;
    }

    private void AddRegionOfInterest()
    {
        if (_dataDetector.RegionOfInterest is null)
        {
            return;
        }

        if (Window is not null)
        {
            Window.DecorView.LayoutChange += DecorView_LayoutChange;
        }

        EventHandler @event = null!;
        @event = (s, e) =>
        {
            FrameLayout frame = FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout));

            _regionOfInterest = new(frame.Context, _dataDetector.RegionOfInterest);

            frame.AddView(_regionOfInterest);

            _regionOfInterest.StartStrokeAnimation();
            ShowEvent -= @event;
        };

        ShowEvent += @event;
    }

    private void DecorView_LayoutChange(object? sender, View.LayoutChangeEventArgs e)
    {
        if (_dataDetector.RegionOfInterest is null)
        {
            return;
        }

        _dataDetector.RegionOfInterest.SetConstraints(Convert.ToInt32(Context.FromPixels(Window?.DecorView.Width ?? 0)), Convert.ToInt32(Context.FromPixels(Window?.DecorView.Height ?? 0)));

        if (IsShowing)
        {
            _regionOfInterest?.Reset();
        }
    }

    private void Detected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        _recognizedItems = e;

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        RecognizedItem? recognizedItem;

        if (_recognizeMultiple == false)
        {
            _selectedRecognizedItem ??= e.First();

            recognizedItem = e.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) is RecognizedItem item ? item : e.First();
        }
        else
        {
            recognizedItem = e.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) is RecognizedItem item ? item : null;
        }

        RecognizedItemButton itemButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = recognizedItem;
        itemButton.Visibility = recognizedItem is null ? ViewStates.Gone : ViewStates.Visible;

        if (_isHighlightingEnabled)
        {
            if (_recognizeMultiple == false)
            {
                if (recognizedItem is not null)
                {
                    AddHighlight(previewView.Overlay, recognizedItem);
                }
            }
            else
            {
                foreach (RecognizedItem item in e)
                {
                    AddHighlight(previewView.Overlay, item);
                }
            }
        }

        previewView.Invalidate();
    }

    private void Cleared(object? sender, EventArgs e)
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        _recognizedItemHighlights.ForEach(x => x.Dispose());
        _recognizedItemHighlights.Clear();

        RecognizedItemButton itemButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = null;
        itemButton.Visibility = ViewStates.Gone;

        previewView.Invalidate();
    }

    private void AddHighlight(ViewOverlay? overlay, RecognizedItem item)
    {
        RecognizedItemHighlight highlight = new(item);

        _recognizedItemHighlights.Add(highlight);
        overlay?.Add(highlight);
    }

    private void RecognizedItemClicked(object? sender, RecognizedItem e)
    {
        _scanCompleteTaskSource?.TrySetResult(e);
        Dismiss();
    }

    private void CloseButtonClicked(object? sender, EventArgs e)
    {
        Cancel();
    }

    private void FlashButton_Toggled(object? sender, int e)
    {
        _cameraController.ImageCaptureFlashMode = e;
        _cameraController.EnableTorch(e != ImageCapture.FlashModeOff);
    }
}
