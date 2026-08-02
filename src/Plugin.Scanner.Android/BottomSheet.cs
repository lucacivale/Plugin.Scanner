using AndroidX.Core.View;
using Google.Android.Material.BottomSheet;

namespace Plugin.Scanner.Android;

internal sealed class BottomSheet : PopupWindow, IOnApplyWindowInsetsListener
{
    private readonly Window? _window;

    private readonly FrameLayout _bottomSheet;
    private readonly View _touchOutside;
    private readonly BottomSheetBehavior _behavior;
    private readonly BottomSheetCallback _bottomSheetCallback;

    private EdgeToEdgeCallback? _edgeToEdgeCallback;

    public BottomSheet(Activity context)
        : base(context)
    {
        _window = context.Window;

        FrameLayout container = ((FrameLayout?)LayoutInflater.FromContext(context)?.Inflate(Resource.Layout.BottomSheetPopup, null)) ?? new FrameLayout(context);
        _bottomSheet = container.FindViewById<FrameLayout>(Resource.Id.design_bottom_sheet) ?? new FrameLayout(context);
        _touchOutside = container.FindViewById<FrameLayout>(Resource.Id.touch_outside) ?? new View(context);

        _bottomSheetCallback = new(this);
        _behavior = BottomSheetBehavior.From(_bottomSheet);
        _behavior.AddBottomSheetCallback(_bottomSheetCallback);

        ViewCompat.SetOnApplyWindowInsetsListener(container, this);

        ContentView = container;
    }

    public bool IsModal
    {
        get => Focusable;
        set
        {
            Focusable = value;

            if (IsModal)
            {
                _touchOutside.Click += TouchOutside_Click;
            }
            else
            {
                _touchOutside.Click -= TouchOutside_Click;
            }
        }
    }

    public bool IsCancelable { get => _behavior.Hideable; set => _behavior.Hideable = value; }

    public bool FitToContent { get => _behavior.FitToContents; set => _behavior.FitToContents = value; }

    public bool Draggable { get => _behavior.Draggable; set => _behavior.Draggable = value; }

    public int State { get => _behavior.State; set => _behavior.State = value; }

    public void SetContentView(View content)
    {
        _bottomSheet.RemoveAllViews();
        _bottomSheet.AddView(content);

        Touchable = true;
        IsModal = true;
        IsCancelable = true;
        FitToContent = false;
        Draggable = true;
        ClippingEnabled = false;
        State = FitToContent ? BottomSheetBehavior.StateExpanded : BottomSheetBehavior.StateHalfExpanded;
        AnimationStyle = _Microsoft.Android.Resource.Designer.Resource.Style.PluginScannerBottomSheetPopupAnimation;
    }

    public void Show(View anchor)
    {
        Width = anchor.Width;
        Height = anchor.Height;

        ShowAtLocation(
            anchor,
            GravityFlags.NoGravity,
            0,
            0);
    }

    public override void Dismiss()
    {
        if (IsCancelable == false)
        {
            return;
        }

        if (_behavior.State == BottomSheetBehavior.StateHidden)
        {
            _touchOutside.Click -= TouchOutside_Click;

            base.Dismiss();
        }
        else
        {
            _behavior.State = BottomSheetBehavior.StateHidden;
        }
    }

    public WindowInsetsCompat? OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
    {
        if (_edgeToEdgeCallback is not null)
        {
            _behavior.RemoveBottomSheetCallback(_edgeToEdgeCallback);

            _edgeToEdgeCallback.Dispose();
            _edgeToEdgeCallback = null;
        }

        if (insets is not null
            && _window is not null)
        {
            _edgeToEdgeCallback = new(_window, _bottomSheet, insets);

            _behavior.AddBottomSheetCallback(_edgeToEdgeCallback);
        }

        return insets;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _behavior.Dispose();
            _bottomSheetCallback.Dispose();
            _edgeToEdgeCallback?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void TouchOutside_Click(object? sender, EventArgs e)
    {
        if (IsCancelable == false)
        {
            return;
        }

        Dismiss();
    }

    private sealed class BottomSheetCallback : BottomSheetBehavior.BottomSheetCallback
    {
        readonly BottomSheet _owner;

        public BottomSheetCallback(BottomSheet owner)
        {
            _owner = owner;
        }

        public override void OnSlide(View bottomSheet, float newState)
        {
        }

        public override void OnStateChanged(View p0, int p1)
        {
            if (p1 == BottomSheetBehavior.StateHidden)
            {
                _owner?.Dismiss();
            }
        }
    }

    private sealed class EdgeToEdgeCallback : BottomSheetBehavior.BottomSheetCallback
    {
        private readonly Window _window;
        private readonly bool? _lightBottomSheet;
        private readonly WindowInsetsCompat _insetsCompat;
        private readonly bool _lightStatusBar;

        public EdgeToEdgeCallback(Window window, View bottomSheet, WindowInsetsCompat insetsCompat)
        {
            _insetsCompat = insetsCompat;
            _window = window;

            _lightStatusBar = WindowCompat.GetInsetsController(window, window.DecorView)?.AppearanceLightStatusBars ?? false;

            if (bottomSheet.Background is ColorDrawable colorDrawable)
            {
                _lightBottomSheet = IsColorLight(colorDrawable.Color);
            }
        }

        public override void OnStateChanged(View p0, int p1)
        {
            SetPaddingForPosition(p0);
        }

        public override void OnSlide(View bottomSheet, float newState)
        {
            SetPaddingForPosition(bottomSheet);
        }

        private static bool IsColorLight(int color)
        {
            if (color == Color.Transparent)
            {
                return false;
            }

            double darkness =
                1 -
                (((0.299 * Color.GetRedComponent(color)) +
                 (0.587 * Color.GetGreenComponent(color)) +
                 (0.114 * Color.GetBlueComponent(color))) / 255);

            return darkness < 0.5;
        }

        private void SetPaddingForPosition(View bottomSheet)
        {
            int insetTop = _insetsCompat.GetInsets(WindowInsetsCompat.Type.SystemBars())?.Top ?? 0;

            if (bottomSheet.Top < insetTop)
            {
                WindowCompat.GetInsetsController(_window, _window.DecorView)?.AppearanceLightStatusBars = _lightBottomSheet ?? _lightStatusBar;

                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    insetTop - bottomSheet.Top,
                    bottomSheet.PaddingRight,
                    bottomSheet.PaddingBottom);
            }
            else if (bottomSheet.Top != 0)
            {
                WindowCompat.GetInsetsController(_window, _window.DecorView)?.AppearanceLightStatusBars = _lightStatusBar;

                bottomSheet.SetPadding(
                    bottomSheet.PaddingLeft,
                    0,
                    bottomSheet.PaddingRight,
                    bottomSheet.PaddingBottom);
            }
        }
    }
}
