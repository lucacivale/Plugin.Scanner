using Vision;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public static class BarcodeScanner
{
    public static async Task<string> ScanBarcodeAsync()
    {
        UIViewController? topViewController = WindowUtils.GetTopViewController();

        if (topViewController is null
            || DataScannerViewController.IsSupported == false
            || DataScannerViewController.IsAvailable == false)
        {
            return string.Empty;
        }

        TaskCompletionSource<string> taskSource = new();

        using RecognizedDataType barcodeType = RecognizedDataType.Barcode(VNDetectBarcodesRequest.SupportedSymbologies);
        using DataScannerViewController scanner = new([barcodeType]);
        scanner.Delegate = new BarcodeScannerDelegate(scanner, barcode => taskSource.TrySetResult(barcode));
        scanner.StartScanning(out DataScannerStartScanningException? error);

        if (error is not null)
        {
            taskSource.TrySetException(error);
        }

        await topViewController.PresentViewControllerAsync(scanner.ScannerViewController, true).ConfigureAwait(true);

        string barcode = await taskSource.Task.WaitAsync(CancellationToken.None).ConfigureAwait(true);

        await scanner.ScannerViewController.DismissViewControllerAsync(true).ConfigureAwait(true);

        return barcode;
    }
}