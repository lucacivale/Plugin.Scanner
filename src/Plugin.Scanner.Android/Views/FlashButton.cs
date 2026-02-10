using System.ComponentModel;
using Android.Util;
using AndroidX.Camera.Core;
using Java.Lang;

namespace Plugin.Scanner.Android.Views;

/// <summary>
/// Represents a toggleable flash button for camera controls that cycles through off, on, and auto modes.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class FlashButton : ImageButton, IRunnable
{
    private readonly int _onResourceId = _Microsoft.Android.Resource.Designer.Resource.Drawable.flash_on_24;
    private readonly int _offResourceId = _Microsoft.Android.Resource.Designer.Resource.Drawable.flash_off_24;
    private readonly int _autoResourceId = _Microsoft.Android.Resource.Designer.Resource.Drawable.flash_auto_24;

    private int _flashMode = ImageCapture.FlashModeOff;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashButton"/> class.
    /// </summary>
    /// <param name="context">The Android context.</param>
    public FlashButton(Context context)
        : base(context)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashButton"/> class with the specified attribute set.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    public FlashButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashButton"/> class with the specified attribute set and style.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    /// <param name="defStyleAttr">The default style attribute.</param>
    public FlashButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    /// <summary>
    /// Occurs when the flash mode is toggled.
    /// </summary>
    /// <value>An event handler that receives the new flash mode value.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "Type should not be visible outside. Only public for Android layout inflation.")]
    public event EventHandler<int>? Toggled;

    /// <summary>
    /// Runs the animation end action to restore the button's appearance after the toggle animation.
    /// </summary>
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

    /// <summary>
    /// Releases the resources used by the <see cref="FlashButton"/>.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= AnimateToggle;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Sets the flash icon based on the current flash mode.
    /// </summary>
    private void SetFlashIcon()
    {
        int iconRes = _flashMode switch
        {
            ImageCapture.FlashModeOff => _onResourceId,
            ImageCapture.FlashModeOn => _autoResourceId,
            _ => _offResourceId,
        };

        SetImageResource(iconRes);
    }

    /// <summary>
    /// Initializes the button by subscribing to click events.
    /// </summary>
    private void Init()
    {
        Click += AnimateToggle;
    }

    /// <summary>
    /// Handles the button click to toggle the flash mode with animation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void AnimateToggle(object? sender, EventArgs e)
    {
        _flashMode = _flashMode switch
        {
            ImageCapture.FlashModeOff => ImageCapture.FlashModeOn,
            ImageCapture.FlashModeOn => ImageCapture.FlashModeAuto,
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
