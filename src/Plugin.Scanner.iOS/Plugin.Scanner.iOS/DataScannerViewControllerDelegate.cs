using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.iOS;

/// <summary>
/// Implements the delegate pattern for <see cref="Binding.DataScannerViewController"/> events,
/// exposing them as standard .NET events.
/// </summary>
internal sealed class DataScannerViewControllerDelegate : iOS.Binding.DataScannerViewControllerDelegate
{
    /// <summary>
    /// Gets or sets the event handler invoked when the scanner zoom level changes.
    /// </summary>
    public EventHandler? Zoomed { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when a recognized item is tapped.
    /// </summary>
    public EventHandler<RecognizedItem>? Tapped { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when items are added to the scanner's recognition results.
    /// </summary>
    public EventHandler<(RecognizedItem[] AddedItems, RecognizedItem[] AllItems)>? Added { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when recognized items are updated.
    /// </summary>
    public EventHandler<(RecognizedItem[] UpdatedItems, RecognizedItem[] AllItems)>? Updated { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when items are removed from the scanner's recognition results.
    /// </summary>
    public EventHandler<(RecognizedItem[] RemovedItems, RecognizedItem[] AllItems)>? Removed { get; set; }

    /// <summary>
    /// Gets or sets the event handler invoked when the scanner becomes unavailable.
    /// </summary>
    public EventHandler<DataScannerUnavailableException>? BecameUnavailable { get; set; }

    /// <summary>
    /// Called when the scanner zoom level changes.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> that changed zoom level.</param>
    public override void DidZoom(iOS.Binding.DataScannerViewController dataScanner)
    {
        Zoomed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the user taps a recognized item.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> containing the tapped item.</param>
    /// <param name="item">The <see cref="RecognizedItem"/> that was tapped.</param>
    public override void DidTapOn(iOS.Binding.DataScannerViewController dataScanner, RecognizedItem item)
    {
        Tapped?.Invoke(this, item);
    }

    /// <summary>
    /// Called when new items are recognized and added to the scanner's results.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> that recognized the items.</param>
    /// <param name="addedItems">The array of newly added <see cref="RecognizedItem"/> objects.</param>
    /// <param name="allItems">The complete array of all currently recognized <see cref="RecognizedItem"/> objects.</param>
    public override void DidAdd(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] addedItems,
        RecognizedItem[] allItems)
    {
        Added?.Invoke(this, (addedItems, allItems));
    }

    /// <summary>
    /// Called when existing recognized items are updated with new information.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> that updated the items.</param>
    /// <param name="updatedItems">The array of updated <see cref="RecognizedItem"/> objects.</param>
    /// <param name="allItems">The complete array of all currently recognized <see cref="RecognizedItem"/> objects.</param>
    public override void DidUpdate(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] updatedItems,
        RecognizedItem[] allItems)
    {
        Updated?.Invoke(this, (updatedItems, allItems));
    }

    /// <summary>
    /// Called when items are removed from the scanner's recognition results.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> that removed the items.</param>
    /// <param name="removedItems">The array of removed <see cref="RecognizedItem"/> objects.</param>
    /// <param name="allItems">The complete array of all currently recognized <see cref="RecognizedItem"/> objects.</param>
    public override void DidRemove(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] removedItems,
        RecognizedItem[] allItems)
    {
        Removed?.Invoke(this, (removedItems, allItems));
    }

    /// <summary>
    /// Called when the scanner becomes unavailable due to an error condition.
    /// </summary>
    /// <param name="dataScanner">The <see cref="Binding.DataScannerViewController"/> that became unavailable.</param>
    /// <param name="error">The <see cref="ScanningUnavailable"/> error code indicating why the scanner became unavailable.</param>
    public override void BecameUnavailableWithError(iOS.Binding.DataScannerViewController dataScanner, ScanningUnavailable error)
    {
        BecameUnavailable?.Invoke(this, new DataScannerUnavailableException(error.ToString()));
    }
}