using AndroidX.AppCompat.App;
using AndroidX.Camera.Core;
using AndroidX.Camera.View;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Views;

namespace Plugin.Scanner.Android;

internal sealed class DataScannerDialog : AppCompatDialog, View.IOnTouchListener
{
    private readonly Activity _activity;
    private readonly LifecycleCameraController _cameraController;
    private readonly DataDetector _dataDetector;
    private readonly List<RecognizedItemHighlight> _barcodeHighlights = [];

    private readonly bool _recognizeMultiple;

    private TaskCompletionSource<RecognizedItem>? _scanCompleteTaskSource;

    public DataScannerDialog(
        Activity context,
        DataDetector detector,
        ImageAnalysis.IAnalyzer analyzer,
        IExecutor executor,
        bool recognizeMultiple)
        : base(context, _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_DataScannerDialog)
    {
        _activity = context;
        _dataDetector = detector;
        _cameraController = new(Context);
        _recognizeMultiple = recognizeMultiple;

        if (_activity is not ILifecycleOwner owner)
        {
            throw new ActivityMustBeILifecycleOwnerException("Activity must implement ILifecycleOwner");
        }

        _cameraController.BindToLifecycle(owner);
        _cameraController.SetImageAnalysisAnalyzer(executor, analyzer);

        SetContentView();
    }

    public async Task<RecognizedItem> ScanAsync(CancellationToken cancellationToken)
    {
        if (Context.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        Show();

        _dataDetector.Added += AddedItems;
        _dataDetector.Removed += RemovedItems;

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
        if (e?.Action == MotionEventActions.Up
            && _barcodeHighlights.FirstOrDefault(x => x.RecognizedItem.Bounds.Contains((int)e.GetX(), (int)e.GetY())) is RecognizedItemHighlight highlight)
        {
            RecognizedItemButton recognizedItemButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));

            if (highlight.RecognizedItem.Text != recognizedItemButton.RecognizedItem?.Text)
            {
                recognizedItemButton.RecognizedItem = highlight.RecognizedItem;
                recognizedItemButton.Visibility = ViewStates.Visible;
            }
        }

        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cameraController.Dispose();
        }

        base.Dispose(disposing);
    }

    protected void Cleanup()
    {
        _dataDetector.Stop();
        _dataDetector.Added -= AddedItems;
        _dataDetector.Removed -= RemovedItems;

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;
        previewView.SetOnTouchListener(null);

        RecognizedItemButton recognizedButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        recognizedButton.Clicked -= RecognizedItemClicked;

        ImageButton closeButton = FindViewById<ImageButton>(_Microsoft.Android.Resource.Designer.Resource.Id.closeButton) ?? throw new ViewNotFoundException(nameof(ImageButton));
        closeButton.Click -= CloseButtonClicked;

        FlashButton flashButton = FindViewById<FlashButton>(_Microsoft.Android.Resource.Designer.Resource.Id.flashButton) ?? throw new ViewNotFoundException(nameof(FlashButton));
        flashButton.Toggled -= FlashButton_Toggled;

        _cameraController.ClearImageAnalysisAnalyzer();
        _cameraController.Unbind();
    }

    private void SetContentView()
    {
        base.SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScanner);

        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;

        if (_recognizeMultiple)
        {
            previewView.SetOnTouchListener(this);
        }

        AddOverlay();
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

    private void AddedItems(object? sender, (IReadOnlyList<RecognizedItem> Added, IReadOnlyList<RecognizedItem> All) e)
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));

        if (_recognizeMultiple == false)
        {
            if (e.Added.Count >= 1)
            {
                RecognizedItem recognizedItem = e.Added.First();
                AddHighlight(previewView.Overlay, recognizedItem);

                RecognizedItemButton itemButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));

                if (recognizedItem.Text != itemButton.RecognizedItem?.Text)
                {
                    itemButton.RecognizedItem = recognizedItem;
                    itemButton.Visibility = ViewStates.Visible;
                }
            }
        }
        else
        {
            foreach (RecognizedItem recognizedItem in e.Added)
            {
                AddHighlight(previewView.Overlay, recognizedItem);
            }
        }

        previewView.Invalidate();
    }

    private void RemovedItems(object? sender, (IReadOnlyList<RecognizedItem> Removed, IReadOnlyList<RecognizedItem> All) e)
    {
        PreviewView previewView = FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));

        if (_recognizeMultiple == false)
        {
            if (e.Removed.Count >= 1)
            {
                RemoveHighlights(previewView.Overlay, e.Removed.First());
            }
        }
        else
        {
            foreach (RecognizedItem recognizedItem in e.Removed)
            {
                RemoveHighlights(previewView.Overlay, recognizedItem);
            }
        }

        RecognizedItemButton itemButton = FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));

        if (e.All.Any(x => x.Text == itemButton.RecognizedItem?.Text) == false)
        {
            itemButton.RecognizedItem = null;
            itemButton.Visibility = ViewStates.Gone;
        }

        previewView.Invalidate();
    }

    private void AddHighlight(ViewOverlay? overlay, RecognizedItem item)
    {
        RecognizedItemHighlight highlight = new(item);

        _barcodeHighlights.Add(highlight);
        overlay?.Add(highlight);
    }

    private void RemoveHighlights(ViewOverlay? overlay, RecognizedItem item)
    {
        if (_barcodeHighlights.FirstOrDefault(x => x.RecognizedItem.Equals(item)) is RecognizedItemHighlight highlight)
        {
            overlay?.Remove(highlight);
            _barcodeHighlights.Remove(highlight);
        }
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
