using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Core.Controllers;

public partial interface IDataScannerController
{
    UIView Overlay { get; }

    /// <summary>
    /// Gets or sets when a recognized item is tapped.
    /// </summary>
    EventHandler<RecognizedItem>? Tapped { get; set; }

    /// <summary>
    /// Gets or sets when new items are recognized and added.
    /// </summary>
    EventHandler<(RecognizedItem[] AddedItems, RecognizedItem[] AllItems)>? Added { get; set; }

    /// <summary>
    /// Gets or sets when recognized items are updated.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    EventHandler<(RecognizedItem[] UpdatedItems, RecognizedItem[] AllItems)>? Updated { get; set; }

    /// <summary>
    /// Gets or sets when recognized items are removed.
    /// </summary>
    EventHandler<(RecognizedItem[] RemovedItems, RecognizedItem[] AllItems)>? Removed { get; set; }

    void Dismiss(string result);
}
