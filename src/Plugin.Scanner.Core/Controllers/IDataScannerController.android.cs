using Plugin.Scanner.Core.Models;
using System.Diagnostics.CodeAnalysis;

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

    bool IsRunning { get; }

    bool IsDialog { get; }

    /// <summary>
    /// Gets or sets when items are detected by the scanner.
    /// </summary>
    EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    /// <summary>
    /// Gets or sets when the detection area is cleared.
    /// </summary>
    EventHandler? Cleared { get; set; }

    void Cancel();

    void Dismiss(RecognizedItem item);

    T? FindViewById<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(int id)
        where T : View;
}
