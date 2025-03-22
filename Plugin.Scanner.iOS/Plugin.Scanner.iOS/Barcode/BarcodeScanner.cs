using Plugin.Scanner.iOS.Exceptions;
using Vision;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <summary>
/// Barcode scanner.
/// </summary>
public static class BarcodeScanner
{
    /// <summary>
    /// Open a modal sheet to scan a single barcode. After barcode detection the sheet is closed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="DataScannerBarcodeScanException">Throws an <see cref="DataScannerBarcodeScanException"/> if no <see cref="UIViewController"/> is found,
    /// <see cref="DataScannerViewController"/> is unavailable or unsupported or the data scanner could not be started.</exception>
    public static Task<string> ScanBarcodeAsync()
    {
        return ScanBarcodeAsync(CancellationToken.None);
    }

    /// <summary>
    /// Open a modal sheet to scan a single barcode. After barcode detection the sheet is closed.
    /// </summary>
    /// <param name="cancellationToken">Cancel task.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="DataScannerBarcodeScanException">Throws an <see cref="DataScannerBarcodeScanException"/> if no <see cref="UIViewController"/> is found,
    /// <see cref="DataScannerViewController"/> is unavailable or unsupported or the data scanner could not be started.</exception>
    public static async Task<string> ScanBarcodeAsync(CancellationToken cancellationToken)
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

        string barcode = await taskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        await scanner.ScannerViewController.DismissViewControllerAsync(true).ConfigureAwait(true);

        overlayView.Dispose();

        return barcode;
    }
}