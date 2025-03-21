using Vision;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public static class BarcodeScanner
{
    public static async Task<string> ScanBarcodeAsync()
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new DataScannerBarcodeScanException("Failed to find top UIViewController.", new NotSupportedException($"{nameof(topViewController)} can not be null."));

        if (DataScannerViewController.IsSupported == false)
        {
            throw new DataScannerBarcodeScanException("Data scanner not supported.", new DataScannerUnsupportedException(string.Join(", ", DataScannerViewController.ScanningUnavailable)));
        }

        if (DataScannerViewController.IsAvailable == false)
        {
            throw new DataScannerBarcodeScanException("Data scanner not available.", new DataScannerUnavailableException(string.Join(", ", DataScannerViewController.ScanningUnavailable)));
        }

        TaskCompletionSource<string> taskSource = new();

        using RecognizedDataType barcodeType = RecognizedDataType.Barcode(VNDetectBarcodesRequest.SupportedSymbologies);
        using DataScannerViewController scanner = new([barcodeType], isGuidanceEnabled: false, isHighlightingEnabled: false);
        scanner.Delegate = new BarcodeScannerDelegate(scanner, barcode => taskSource.TrySetResult(barcode));

        UIView overlayView = scanner.AddBarcodeRegionOfInterestOverlay();

        await topViewController.PresentViewControllerAsync(scanner.ScannerViewController, true).ConfigureAwait(true);

        scanner.StartScanning(out DataScannerStartException? error);
        if (error is not null)
        {
            taskSource.TrySetException(new DataScannerBarcodeScanException("Could not start scanner.", error));
        }

        string barcode = await taskSource.Task.WaitAsync(CancellationToken.None).ConfigureAwait(true);

        await scanner.ScannerViewController.DismissViewControllerAsync(true).ConfigureAwait(true);

        overlayView.Dispose();

        return barcode;
    }
}