using AVFoundation;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.Views.iOS;

/// <summary>
/// A button that toggles the camera torch between off, on, and auto modes.
/// </summary>
internal sealed class DataScannerTorchButton : ToggleButton<AVCaptureTorchMode>
{
    private const string OnSymbolName = "bolt.fill";
    private const string OffSymbolName = "bolt.slash.fill";
    private const string AutoSymbolName = "bolt.badge.a.fill";

    private AVCaptureTorchMode _torchMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerTorchButton"/> class.
    /// </summary>
    public DataScannerTorchButton()
        : base(OnSymbolName)
    {
    }

    protected override void Toggle(object? sender, EventArgs e)
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

        AnimateIconToggle(symbolName);
    }
}
