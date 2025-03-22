using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// A delegate object that responds when people interact with items that the data scanner recognizes.
/// </summary>
[SuppressMessage("Naming", "CA1711: Identifiers should not have incorrect suffix", Justification = "Is ok here.")]
public abstract class DataScannerViewControllerDelegate : Plugin.Scanner.iOS.Binding.DataScannerViewControllerDelegate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewControllerDelegate"/> class.
    /// </summary>
    /// <param name="scannerViewController">Associated <see cref="DataScannerViewController"/>.</param>
    protected DataScannerViewControllerDelegate(DataScannerViewController scannerViewController)
    {
        DataScannerViewController = scannerViewController;
    }

    /// <summary>
    /// Gets data scanner.
    /// </summary>
    public DataScannerViewController DataScannerViewController { get; }

    /// <summary>
    /// Responds when a person or your code changes the zoom factor.
    /// </summary>
    /// <param name="dataScanner">The data scanner whose zoom factor changes.</param>
    public override void DataScannerDidZoom(Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner)
    {
        DataScannerDidZoom(DataScannerViewController);
    }

    /// <summary>
    /// Responds when a person taps an item that the data scanner recognizes.
    /// </summary>
    /// <param name="dataScanner">The data scanner whose recognized item is tapped.</param>
    /// <param name="item">The item that a person taps.</param>
    public override void DidTapOn(Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner, Plugin.Scanner.iOS.Binding.RecognizedItem item)
    {
        DidTapOn(DataScannerViewController, item.ToRecognizedItem());
    }

    /// <summary>
    /// Responds when the data scanner starts recognizing an item.
    /// </summary>
    /// <param name="dataScanner"> The data scanner that recognizes the item.</param>
    /// <param name="addedItems">The items that the data scanner starts tracking.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public override void DidAdd(
        Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] addedItems,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] allItems)
    {
        DidAdd(
            DataScannerViewController,
            addedItems.Select(x => x.ToRecognizedItem()).ToArray(),
            allItems.Select(x => x.ToRecognizedItem()).ToArray());
    }

    /// <summary>
    /// Responds when the data scanner updates the geometry of an item it recognizes.
    /// </summary>
    /// <param name="dataScanner">The data scanner that recognizes the item.</param>
    /// <param name="updatedItems">The items with geometry that the data scanner changes.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public override void DidUpdate(
        Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] updatedItems,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] allItems)
    {
        DidUpdate(
            DataScannerViewController,
            updatedItems.Select(x => x.ToRecognizedItem()).ToArray(),
            allItems.Select(x => x.ToRecognizedItem()).ToArray());
    }

    /// <summary>
    /// Responds when the data scanner stops recognizing an item.
    /// </summary>
    /// <param name="dataScanner">The data scanner that recognizes the item.</param>
    /// <param name="removedItems">The items that the data scanner removes.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public override void DidRemove(
        Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] removedItems,
        Plugin.Scanner.iOS.Binding.RecognizedItem[] allItems)
    {
        DidRemove(
            DataScannerViewController,
            removedItems.Select(x => x.ToRecognizedItem()).ToArray(),
            allItems.Select(x => x.ToRecognizedItem()).ToArray());
    }

    /// <summary>
    /// Responds when the data scanner becomes unavailable and stops scanning.
    /// </summary>
    /// <param name="dataScanner">The data scanner that’s not available.</param>
    /// <param name="error">Describes an error if it occurs.</param>
    public override void BecameUnavailableWithError(Plugin.Scanner.iOS.Binding.DataScannerViewController dataScanner, Plugin.Scanner.iOS.Binding.ScanningUnavailable error)
    {
        BecameUnavailableWithError(
            DataScannerViewController,
            error.ToVnScanningUnavailable());
    }

    /// <summary>
    /// Responds when a person or your code changes the zoom factor.
    /// </summary>
    /// <param name="dataScanner">The data scanner whose zoom factor changes.</param>
    public abstract void DataScannerDidZoom(DataScannerViewController dataScanner);

    /// <summary>
    /// Responds when a person taps an item that the data scanner recognizes.
    /// </summary>
    /// <param name="dataScanner">The data scanner with the zoom factor that changes.</param>
    /// <param name="item">The item that a person taps.</param>
    public abstract void DidTapOn(DataScannerViewController dataScanner, RecognizedItem item);

    /// <summary>
    /// Responds when the data scanner starts recognizing an item.
    /// </summary>
    /// <param name="dataScanner">The data scanner that recognizes the item.</param>
    /// <param name="addedItems">The items that the data scanner starts tracking.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public abstract void DidAdd(DataScannerViewController dataScanner, RecognizedItem[] addedItems, RecognizedItem[] allItems);

    /// <summary>
    /// Responds when the data scanner updates the geometry of an item it recognizes.
    /// </summary>
    /// <param name="dataScanner">The data scanner that recognizes the item.</param>
    /// <param name="updatedItems">The items with geometry that the data scanner changes.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public abstract void DidUpdate(DataScannerViewController dataScanner, RecognizedItem[] updatedItems, RecognizedItem[] allItems);

    /// <summary>
    /// To identify an item in the `removedItems` and `allItems` parameters, use the item’s `id` property.
    /// </summary>
    /// <param name="dataScanner">The data scanner that recognizes the item.</param>
    /// <param name="removedItems">The items that the data scanner removes from the ``DataScannerViewController/recognizedItems`` property.</param>
    /// <param name="allItems">The current items that the data scanner tracks. Text items appear in the reading order of the language and region.</param>
    public abstract void DidRemove(DataScannerViewController dataScanner, RecognizedItem[] removedItems, RecognizedItem[] allItems);

    /// <summary>
    /// Responds when the data scanner becomes unavailable and stops scanning.
    /// </summary>
    /// <param name="dataScanner">The data scanner that’s not available.</param>
    /// <param name="error">Describes an error if it occurs.</param>
    [SuppressMessage("Naming", "CA1716: Identifiers should not match keywords", Justification = "Is ok here.")]
    public abstract void BecameUnavailableWithError(DataScannerViewController dataScanner, ScanningUnavailable error);
}
