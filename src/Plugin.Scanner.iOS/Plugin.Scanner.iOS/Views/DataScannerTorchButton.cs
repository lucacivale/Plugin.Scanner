using AVFoundation;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.iOS.Views;

/// <summary>
/// A button control that toggles the device's torch (flashlight) through different modes with animated visual feedback.
/// </summary>
/// <remarks>
/// <para>
/// The button cycles through three torch modes: Off → On → Auto → Off.
/// Each mode is represented by a different SF Symbol icon.
/// </para>
/// <para>
/// The button provides visual feedback through scale and cross-dissolve animations when toggled.
/// </para>
/// </remarks>
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
    /// <remarks>
    /// The button is initialized with the torch off state, white tint color, and Auto Layout enabled.
    /// </remarks>
    public DataScannerTorchButton()
    {
        _torchButtonSymbolConfiguration = UIImageSymbolConfiguration.Create(22, UIImageSymbolWeight.Medium);

        SetImage(UIImage.GetSystemImage(OffSymbolName, _torchButtonSymbolConfiguration), UIControlState.Normal);

        TintColor = UIColor.White;
        TranslatesAutoresizingMaskIntoConstraints = false;

        TouchUpInside += ToggleTorchMode;
    }

    /// <summary>
    /// Gets or sets the event handler invoked when the torch mode is toggled.
    /// </summary>
    /// <value>
    /// An event handler that receives the new <see cref="AVCaptureTorchMode"/> value.
    /// </value>
    public EventHandler<AVCaptureTorchMode>? Toggled { get; set; }

    /// <summary>
    /// Releases the unmanaged resources used by the button and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Cleanup();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Toggles the torch mode to the next state and updates the button's visual appearance.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <exception cref="DataScannerTorchModeUnsupportedException">Thrown when an unsupported torch mode is encountered.</exception>
    /// <remarks>
    /// <para>
    /// The torch mode cycles through the following sequence:
    /// <list type="bullet">
    /// <item><description>Off → On</description></item>
    /// <item><description>On → Auto</description></item>
    /// <item><description>Auto → Off</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The button animates with a scale-down effect followed by a cross-dissolve transition when the icon changes.
    /// </para>
    /// </remarks>
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
            AVCaptureTorchMode.Off => OffSymbolName,
            AVCaptureTorchMode.On => OnSymbolName,
            AVCaptureTorchMode.Auto => AutoSymbolName,
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
    /// Cleans up managed resources used by the button.
    /// </summary>
    private void Cleanup()
    {
        _torchButtonSymbolConfiguration.Dispose();
    }
}