using System.ComponentModel;
using Android.Util;
using Java.Lang;

namespace Plugin.Scanner.Views.Android;

/// <summary>
/// Represents a toggleable expand minimize button for camera controls that cycles through off, on, and auto modes.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ExpandMinimizeButton : ImageButton, IRunnable
{
    private readonly int _expandResourceId = _Microsoft.Android.Resource.Designer.Resource.Drawable.expand_24;
    private readonly int _minimizeResourceId = _Microsoft.Android.Resource.Designer.Resource.Drawable.minimize_24;

    private bool _isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandMinimizeButton"/> class.
    /// </summary>
    /// <param name="context">The Android context.</param>
    public ExpandMinimizeButton(Context context)
        : base(context)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandMinimizeButton"/> class with the specified attribute set.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    public ExpandMinimizeButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandMinimizeButton"/> class with the specified attribute set and style.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    /// <param name="defStyleAttr">The default style attribute.</param>
    public ExpandMinimizeButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    /// <summary>
    /// Occurs when the expand minimize mode is toggled.
    /// </summary>
    /// <value>An event handler that receives the new flash mode value.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "Type should not be visible outside. Only public for Android layout inflation.")]
    public event EventHandler<bool>? Toggled;

    /// <summary>
    /// Runs the animation end action to restore the button's appearance after the toggle animation.
    /// </summary>
    public void Run()
    {
        SetIcon();

        Animate()?
            .ScaleX(1f)
            .ScaleY(1f)
            .Alpha(1f)
            .SetDuration(150)
            .Start();
    }

    /// <summary>
    /// Releases the resources used by the <see cref="ExpandMinimizeButton"/>.
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
    private void SetIcon()
    {
        int iconRes = _isExpanded ? _minimizeResourceId : _expandResourceId;

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
        _isExpanded = !_isExpanded;

        Toggled?.Invoke(this, _isExpanded);

        Animate()?
            .ScaleX(0.9f)
            .ScaleY(0.9f)
            .Alpha(0f)
            .SetDuration(100)
            .WithEndAction(this)
            .Start();
    }
}
