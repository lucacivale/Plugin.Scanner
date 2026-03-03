using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.iOS;

/// <summary>
/// Delegate implementation for iOS DataScannerViewController that handles scanning events.
/// </summary>
internal sealed class DataScannerViewControllerDelegate : iOS.Binding.DataScannerViewControllerDelegate
{
    /// <summary>
    /// Gets or sets when the scanner zoom level changes.
    /// </summary>
    public EventHandler? Zoomed { get; set; }

    /// <summary>
    /// Gets or sets when a recognized item is tapped.
    /// </summary>
    public EventHandler<RecognizedItem>? Tapped { get; set; }

    /// <summary>
    /// Gets or sets when new items are recognized and added to the scanning session.
    /// </summary>
    public EventHandler<(RecognizedItem[] AddedItems, RecognizedItem[] AllItems)>? Added { get; set; }

    /// <summary>
    /// Gets or sets when existing recognized items are updated.
    /// </summary>
    public EventHandler<(RecognizedItem[] UpdatedItems, RecognizedItem[] AllItems)>? Updated { get; set; }

    /// <summary>
    /// Gets or sets when recognized items are removed from the scanning session.
    /// </summary>
    public EventHandler<(RecognizedItem[] RemovedItems, RecognizedItem[] AllItems)>? Removed { get; set; }

    /// <summary>
    /// Gets or sets when the data scanner becomes unavailable.
    /// </summary>
    public EventHandler<DataScannerUnavailableException>? BecameUnavailable { get; set; }

    /// <summary>
    /// Called when the scanner zoom level changes and raises the <see cref="Zoomed"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    public override void DidZoom(iOS.Binding.DataScannerViewController dataScanner)
    {
        Zoomed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when a recognized item is tapped and raises the <see cref="Tapped"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    /// <param name="item">The tapped recognized item.</param>
    public override void DidTapOn(iOS.Binding.DataScannerViewController dataScanner, RecognizedItem item)
    {
        Tapped?.Invoke(this, item);
    }

    /// <summary>
    /// Called when new items are added to the scanning session and raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    /// <param name="addedItems">The newly recognized items.</param>
    /// <param name="allItems">All currently recognized items.</param>
    public override void DidAdd(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] addedItems,
        RecognizedItem[] allItems)
    {
        Added?.Invoke(this, (addedItems, allItems));
    }

    /// <summary>
    /// Called when recognized items are updated and raises the <see cref="Updated"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    /// <param name="updatedItems">The updated recognized items.</param>
    /// <param name="allItems">All currently recognized items.</param>
    public override void DidUpdate(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] updatedItems,
        RecognizedItem[] allItems)
    {
        Updated?.Invoke(this, (updatedItems, allItems));
    }

    /// <summary>
    /// Called when recognized items are removed from the scanning session and raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    /// <param name="removedItems">The removed recognized items.</param>
    /// <param name="allItems">All currently recognized items.</param>
    public override void DidRemove(
        iOS.Binding.DataScannerViewController dataScanner,
        RecognizedItem[] removedItems,
        RecognizedItem[] allItems)
    {
        Removed?.Invoke(this, (removedItems, allItems));
    }

    /// <summary>
    /// Called when the scanner becomes unavailable and raises the <see cref="BecameUnavailable"/> event.
    /// </summary>
    /// <param name="dataScanner">The data scanner view controller.</param>
    /// <param name="error">The error indicating why scanning became unavailable.</param>
    public override void BecameUnavailableWithError(iOS.Binding.DataScannerViewController dataScanner, ScanningUnavailable error)
    {
        BecameUnavailable?.Invoke(this, new DataScannerUnavailableException(error.ToString()));
    }
}