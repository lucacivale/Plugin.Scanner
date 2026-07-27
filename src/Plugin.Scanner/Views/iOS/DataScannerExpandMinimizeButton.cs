namespace Plugin.Scanner.Views.iOS;

/// <summary>
/// A button that toggles the camera torch between off, on, and auto modes.
/// </summary>
internal sealed class DataScannerExpandMinimizeButton : ToggleButton<bool>
{
    private const string ExpandSymbolName = "arrow.down.left.and.arrow.up.right.square";
    private const string MinimizeSymbolName = "arrow.up.right.and.arrow.down.left.square";

    private bool _isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerExpandMinimizeButton"/> class.
    /// </summary>
    public DataScannerExpandMinimizeButton()
        : base(ExpandSymbolName)
    {
    }

    protected override void Toggle(object? sender, EventArgs e)
    {
        _isExpanded = !_isExpanded;

        string symbolName = _isExpanded ? MinimizeSymbolName : ExpandSymbolName;

        Toggled?.Invoke(this, _isExpanded);

        AnimateIconToggle(symbolName);
    }
}
