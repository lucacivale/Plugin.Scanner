using AVFoundation;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.Views.iOS;

/// <summary>
/// A button that toggles the camera torch between off, on, and auto modes.
/// </summary>
internal sealed class DataScannerTorchButton : UIButton
{
    private const string OnSymbolName = "bolt.fill";
    private const string OffSymbolName = "bolt.slash.fill";
    private const string AutoSymbolName = "bolt.badge.a.fill";

    private readonly UIImageSymbolConfiguration _torchButtonSymbolConfiguration;

    private AVCaptureTorchMode _torchMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerTorchButton"/> class.
    /// </summary>
    public DataScannerTorchButton()
    {
        _torchButtonSymbolConfiguration = UIImageSymbolConfiguration.Create(22, UIImageSymbolWeight.Medium);

        SetImage(UIImage.GetSystemImage(OnSymbolName, _torchButtonSymbolConfiguration), UIControlState.Normal);

        TintColor = UIColor.White;
        TranslatesAutoresizingMaskIntoConstraints = false;

        TouchUpInside += ToggleTorchMode;
    }

    /// <summary>
    /// Gets or sets when the torch mode is toggled.
    /// </summary>
    public EventHandler<AVCaptureTorchMode>? Toggled { get; set; }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cleanup();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Toggles the torch mode and updates the button icon with animation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void ToggleTorchMode(object? sender, EventArgs e)
    {
        _torchMode = _torchMode switch
        {
            AVCaptureTorchMode.Off => AVCaptureTorchMode.On,
            AVCaptureTorchMode.On => AVCaptureTorchMode.Auto,
            AVCaptureTorchMode.Auto => AVCaptureTorchMode.Off,
            _ => throw new DataScannerTorchModeUnsupportedException($"Torch mode {_torchMode} is not supported."),
        };

        string symbolName = _torchMode switch
        {
            AVCaptureTorchMode.Off => OnSymbolName,
            AVCaptureTorchMode.On => AutoSymbolName,
            _ => OffSymbolName,
        };

        Toggled?.Invoke(this, _torchMode);

        Animate(0.1, () => Transform = CGAffineTransform.MakeScale(0.9f, 0.9f));
        Transition(
            this,
            0.15,
            UIViewAnimationOptions.TransitionCrossDissolve,
            () =>
            {
                SetImage(
                    UIImage.GetSystemImage(symbolName, _torchButtonSymbolConfiguration),
                    UIControlState.Normal);
                Transform = CGAffineTransform.MakeIdentity();
            },
            null!);
    }

    /// <summary>
    /// Cleans up resources used by the button.
    /// </summary>
    private void Cleanup()
    {
        _torchButtonSymbolConfiguration.Dispose();
    }
}