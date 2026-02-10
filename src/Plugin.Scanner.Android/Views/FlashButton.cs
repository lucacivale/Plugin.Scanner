using Android.Util;
using AndroidX.Annotations;
using AndroidX.Camera.Core;
using Java.Lang;

namespace Plugin.Scanner.Android.Views;

public sealed class FlashButton : ImageButton, IRunnable
{
    private readonly int _onResourceId = Resource.Drawable.flash_on_24;
    private readonly int _offResourceId = Resource.Drawable.flash_off_24;
    private readonly int _autoResourceId = Resource.Drawable.flash_auto_24;

    private int _flashMode = ImageCapture.FlashModeOff;

    public FlashButton(Context context)
        : base(context)
    {
        Init();
    }

    public FlashButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    public FlashButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    public event EventHandler<int> Toggled;

    public void Run()
    {
        SetFlashIcon();

        Animate()?
            .ScaleX(1f)
            .ScaleY(1f)
            .Alpha(1f)
            .SetDuration(150)
            .Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= AnimateToggle;
        }

        base.Dispose(disposing);
    }

    private void SetFlashIcon()
    {
        int iconRes = _flashMode switch
        {
            ImageCapture.FlashModeOff => _onResourceId,
            ImageCapture.FlashModeOn => _autoResourceId,
            ImageCapture.FlashModeAuto => _offResourceId,
            _ => _offResourceId,
        };

        SetImageResource(iconRes);
    }

    private void Init()
    {
        Click += AnimateToggle;
    }

    private void AnimateToggle(object? sender, EventArgs e)
    {
        _flashMode = _flashMode switch
        {
            ImageCapture.FlashModeOff => ImageCapture.FlashModeOn,
            ImageCapture.FlashModeOn => ImageCapture.FlashModeAuto,
            ImageCapture.FlashModeAuto => ImageCapture.FlashModeOff,
            _ => ImageCapture.FlashModeOff,
        };

        Toggled?.Invoke(this, _flashMode);

        Animate()?
            .ScaleX(0.9f)
            .ScaleY(0.9f)
            .Alpha(0f)
            .SetDuration(100)
            .WithEndAction(this)
            .Start();
    }
}
