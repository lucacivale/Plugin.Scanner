namespace Plugin.Scanner.Core.Controllers;

public partial interface IDataScannerController
{
    /// <summary>
    /// Gets a value indicating whether multiple items can be recognized simultaneously.
    /// </summary>
    bool RecognizeMultiple { get; }

    /// <summary>
    /// Gets a value indicating whether detected items should be highlighted.
    /// </summary>
    bool IsHighlightingEnabled { get; }

    bool IsDialog { get; }

    void Expand();

    void Minimize();
}
