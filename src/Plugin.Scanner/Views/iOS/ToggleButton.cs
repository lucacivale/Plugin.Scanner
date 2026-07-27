namespace Plugin.Scanner.Views.iOS;

internal abstract class ToggleButton<TToggleParam> : IconButton
{
    protected ToggleButton(string image)
        : base(image)
    {
        TouchUpInside += Toggle;
    }

    /// <summary>
    /// Gets or sets when the torch mode is toggled.
    /// </summary>
    public EventHandler<TToggleParam>? Toggled { get; set; }

    protected abstract void Toggle(object? sender, EventArgs e);

    protected void AnimateIconToggle(string toIcon)
    {
        Animate(0.1, () => Transform = CGAffineTransform.MakeScale(0.9f, 0.9f));
        Transition(
            this,
            0.15,
            UIViewAnimationOptions.TransitionCrossDissolve,
            () =>
            {
                SetImage(
                    UIImage.GetSystemImage(toIcon, _buttonSymbolConfiguration),
                    UIControlState.Normal);
                Transform = CGAffineTransform.MakeIdentity();
            },
            null!);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            TouchUpInside -= Toggle;
        }
    }
}
