using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

internal sealed class BarcodeScannerDelegate : DataScannerViewControllerDelegate
{
    private readonly Action<string> _onScanResult;

    internal BarcodeScannerDelegate(DataScannerViewController dataScannerViewController, Action<string> onScanResult)
        : base(dataScannerViewController)
    {
        _onScanResult = onScanResult;
    }

    public override void DataScannerDidZoom(DataScannerViewController dataScanner)
    {
    }

    public override void DidTapOn(DataScannerViewController dataScanner, RecognizedItem item)
    {
    }

    [SuppressMessage("Usage", "S6608: Prefer indexing instead of Enumerable methods on types implementing IList", Justification = "Improve readability.")]
    public override void DidAdd(DataScannerViewController dataScanner, RecognizedItem[] addedItems, RecognizedItem[] allItems)
    {
        _onScanResult(addedItems.First().Value);
    }

    public override void DidUpdate(DataScannerViewController dataScanner, RecognizedItem[] updatedItems, RecognizedItem[] allItems)
    {
    }

    public override void DidRemove(DataScannerViewController dataScanner, RecognizedItem[] removedItems, RecognizedItem[] allItems)
    {
    }

    public override void BecameUnavailableWithError(DataScannerViewController dataScanner, ScanningUnavailable error)
    {
    }
}