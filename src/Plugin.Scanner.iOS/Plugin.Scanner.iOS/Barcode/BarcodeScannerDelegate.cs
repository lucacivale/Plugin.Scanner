using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <inheritdoc />
internal sealed class BarcodeScannerDelegate : DataScannerViewControllerDelegate
{
    private readonly Action<string> _onScanResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScannerDelegate"/> class.
    /// </summary>
    /// <param name="dataScannerViewController">Associated <see cref="DataScannerViewController"/>.</param>
    /// <param name="onScanResult">Action to execute on captured barcode.</param>
    internal BarcodeScannerDelegate(DataScannerViewController dataScannerViewController, Action<string> onScanResult)
        : base(dataScannerViewController)
    {
        this._onScanResult = onScanResult;
    }

    /// <inheritdoc/>
    public override void DataScannerDidZoom(DataScannerViewController dataScanner)
    {
    }

    /// <inheritdoc/>
    public override void DidTapOn(DataScannerViewController dataScanner, RecognizedItem item)
    {
    }

    /// <inheritdoc/>
    [SuppressMessage("Usage", "S6608: Prefer indexing instead of Enumerable methods on types implementing IList", Justification = "Improve readability.")]
    public override void DidAdd(DataScannerViewController dataScanner, RecognizedItem[] addedItems, RecognizedItem[] allItems)
    {
        this._onScanResult(addedItems.First().Value);
    }

    /// <inheritdoc/>
    public override void DidUpdate(DataScannerViewController dataScanner, RecognizedItem[] updatedItems, RecognizedItem[] allItems)
    {
    }

    /// <inheritdoc/>
    public override void DidRemove(DataScannerViewController dataScanner, RecognizedItem[] removedItems, RecognizedItem[] allItems)
    {
    }

    /// <inheritdoc/>
    public override void BecameUnavailableWithError(DataScannerViewController dataScanner, ScanningUnavailable error)
    {
    }
}