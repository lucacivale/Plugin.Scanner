using Android.Animation;
using Android.Runtime;
using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Controllers;
using Plugin.Scanner.Core.Models;
using System.Diagnostics.CodeAnalysis;
using AndroidX.Activity;
using AndroidX.AppCompat.App;

namespace Plugin.Scanner.Android;

internal sealed class DataScannerPopup : PopupWindow, IDataScannerController, View.IOnTouchListener
{
    private readonly ICurrentActivity _currentActivity;
    private readonly Context _context;
    private readonly LifecycleCameraController _cameraController;
    private readonly IDataDetector _dataDetector;
    private readonly View _parent;

    private readonly bool _recognizeMultiple;
    private readonly bool _isHighlightingEnabled;
    private readonly IOverlay? _overlay;
    private readonly IRegionOfInterest? _regionOfInterest;

    private readonly int _touchSlop;

    private readonly int _minimizedWidth;
    private readonly int _minimizedHeight;

    private BackPressed? _backPressed;

    private bool _dragging;
    private float _startRawX;
    private float _startRawY;
    private int _popupX;
    private int _popupY;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerPopup"/> class.
    /// </summary>
    /// <param name="parent">Popup parent.</param>
    /// <param name="detector">The data detector to use for recognition.</param>
    /// <param name="cameraController">The camera controller for managing camera operations.</param>
    /// <param name="regionOfInterest">Optional region of interest to limit scanning area.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    /// <param name="recognizeMultiple">Whether to recognize multiple items.</param>
    /// <param name="isHighlightingEnabled">Whether to highlight detected items.</param>
    public DataScannerPopup(
        ICurrentActivity currentActivity,
        View parent,
        IDataDetector detector,
        LifecycleCameraController cameraController,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(parent, parent.Width / 2, parent.Height / 3, false)
    {
        ArgumentNullException.ThrowIfNull(parent.Context);

        _parent = parent;
        _context = parent.Context;
        _currentActivity = currentActivity;

        _touchSlop = ViewConfiguration.Get(_context)?.ScaledTouchSlop ?? 0;

        _dataDetector = detector;
        _cameraController = cameraController;
        _regionOfInterest = regionOfInterest;
        _overlay = overlay;

        _recognizeMultiple = recognizeMultiple;
        _isHighlightingEnabled = isHighlightingEnabled;

        _minimizedWidth = Width;
        _minimizedHeight = Height;

        SetContentView();
        SetTouchInterceptor(this);

        AnimationStyle = _Microsoft.Android.Resource.Designer.Resource.Style.PluginScannerPopupAnimation;
        SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
    }

    /// <summary>
    /// Gets or sets when items are detected by the scanner.
    /// </summary>
    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    /// <summary>
    /// Gets or sets when the detection area is cleared.
    /// </summary>
    public EventHandler? Cleared { get; set; }

    /// <summary>
    /// Gets a value indicating whether multiple items can be recognized simultaneously.
    /// </summary>
    public bool RecognizeMultiple => _recognizeMultiple;

    /// <summary>
    /// Gets a value indicating whether detected items should be highlighted.
    /// </summary>
    public bool IsHighlightingEnabled => _isHighlightingEnabled;

    public bool IsRunning => IsShowing;

    public bool IsDialog => false;

    public void Show()
    {
        if (_currentActivity.Activity is AppCompatActivity activity)
        {
            _backPressed = new BackPressed(true);
            _backPressed.OnBackPressed += OnBackPressed;

            activity.OnBackPressedDispatcher.AddCallback(_backPressed);
        }

        int x = _parent.Width - Width;
        int y = 0;

        ShowAtLocation(
            _parent,
            GravityFlags.NoGravity,
            x,
            y);

        _popupX = x;
        _popupY = y;
    }

    public void Cancel()
    {
        Dismiss();
    }

    public void Dismiss(RecognizedItem item)
    {
        Dismiss();
    }

    public override void Dismiss()
    {
        Cleanup();

        base.Dismiss();
    }

    public void Expand()
    {
        int startWidth = Width;
        int endWidth = _parent.Width;

        int startHeight = Height;
        int endHeight = _parent.Height / 2;

        UpdateSize(startWidth, endWidth, startHeight, endHeight);
    }

    public void Minimize()
    {
        int startWidth = Width;
        int endWidth = _minimizedWidth;

        int startHeight = Height;
        int endHeight = _minimizedHeight;

        UpdateSize(startWidth, endWidth, startHeight, endHeight);
    }

    public override void ShowAtLocation(View? parent, [GeneratedEnum] GravityFlags gravity, int x, int y)
    {
        if (_context?.HasCamera() == false)
        {
            throw new NoCameraException("Device has no camera.");
        }

        _dataDetector.Detected += OnDetected;
        _dataDetector.Cleared += OnCleared;

        base.ShowAtLocation(parent, gravity, x, y);
    }

    public T? FindViewById<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(int id) where T : View
    {
        return ContentView?.FindViewById<T>(id);
    }

    public bool OnTouch(View? v, MotionEvent? e)
    {
        bool handled = false;

        switch (e?.ActionMasked)
        {
            case MotionEventActions.Down:
                _startRawX = e.RawX;
                _startRawY = e.RawY;
                _dragging = false;
                break;

            case MotionEventActions.Move:
                float dx = e.RawX - _startRawX;
                float dy = e.RawY - _startRawY;

                _dragging = !_dragging
                    && (Math.Abs(dx) > _touchSlop
                        || Math.Abs(dy) > _touchSlop);

                if (_dragging)
                {
                    _popupX += (int)dx;
                    _popupY += (int)dy;

                    Update(_popupX, _popupY, -1, -1);

                    _startRawX = e.RawX;
                    _startRawY = e.RawY;

                    handled = true;
                }
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                handled = _dragging;
                _dragging = false;
                break;
        }

        return handled;
    }

    /// <summary>
    /// Cleans up scanner resources, detaches event handlers, and removes the overlay.
    /// </summary>
    private void Cleanup()
    {
        _backPressed?.OnBackPressed -= OnBackPressed;
        _backPressed?.Enabled = false;
        _backPressed?.Remove();
        _backPressed?.Dispose();

        _dataDetector.Stop();
        _dataDetector.Detected -= OnDetected;
        _dataDetector.Cleared -= OnCleared;

        PreviewView previewView = ContentView?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = null;

        _overlay?.Cleanup();
    }

    /// <summary>
    /// Initializes and configures the scanner dialog's content view with camera preview, overlay, and region of interest.
    /// </summary>
    private void SetContentView()
    {
        ContentView = LayoutInflater.FromContext(_context)?.Inflate(_Microsoft.Android.Resource.Designer.Resource.Layout.DataScanner, null);

        GradientDrawable background = new();

        background.SetColor(Color.Black);
        background.SetCornerRadius(_context.ToPixels(24));

        ContentView?.Background = background;
        ContentView?.ClipToOutline = true;
        ContentView?.OutlineProvider = ViewOutlineProvider.Background;

        PreviewView previewView = ContentView?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Controller = _cameraController;
        previewView.SetImplementationMode(PreviewView.ImplementationMode.Compatible ?? throw new ViewNotFoundException(nameof(PreviewView.ImplementationMode.Compatible)));

        _overlay?.Init(this, ContentView?.FindViewById<FrameLayout>(_Microsoft.Android.Resource.Designer.Resource.Id.dataScanner) ?? throw new ViewNotFoundException(nameof(FrameLayout)));
        _overlay?.AddOverlay();

        if (_regionOfInterest is not null)
        {
            EventHandler<View.LayoutChangeEventArgs> @event = null!;
            @event = (_, _) =>
            {
                _regionOfInterest?.SetConstraints(Convert.ToInt32(_context.FromPixels(Width)), Convert.ToInt32(_context.FromPixels(Height)));
                _dataDetector.RegionOfInterest = _regionOfInterest?.CalculateRegionOfInterest().ToRectPixel(_context);

                _overlay?.AddRegionOfInterest(_regionOfInterest);

                ContentView.LayoutChange -= @event;
            };

            ContentView.LayoutChange += @event;
        }
    }

    /// <summary>
    /// Handles the data detector's Detected event and forwards it to subscribers.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The list of recognized items.</param>
    private void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        Detected?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the data detector's Cleared event and forwards it to subscribers.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnCleared(object? sender, EventArgs e)
    {
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateSize(
        int startWidth,
        int endWidth,
        int startHeight,
        int endHeight)
    {
        ValueAnimator? animator = ValueAnimator.OfFloat(0f, 1f);

        animator?.SetDuration(250);
        animator?.Update += (s, e) =>
        {
            float progress = (float)e.Animation.AnimatedValue!;

            Width = startWidth + (int)((endWidth - startWidth) * progress);
            Height = startHeight + (int)((endHeight - startHeight) * progress);
            Update(Width, Height);
        };

        animator?.Start();
    }

    private void OnBackPressed(object? sender, EventArgs e)
    {
        Dismiss();
    }
}

[SuppressMessage("Documentation Rules", "SA1402:File may only contain a single type", Justification = "Is okay here.")]
internal sealed class BackPressed : OnBackPressedCallback
{
    public BackPressed(bool enabled)
        : base(enabled)
    {
    }

    public EventHandler? OnBackPressed { get; set; }

    public override void HandleOnBackPressed()
    {
        OnBackPressed?.Invoke(this, EventArgs.Empty);
    }
}
