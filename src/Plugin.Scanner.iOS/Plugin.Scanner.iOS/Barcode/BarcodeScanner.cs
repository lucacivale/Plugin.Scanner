using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.iOS.Exceptions;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <summary>
/// Barcode scanner.
/// </summary>
public sealed class BarcodeScanner : IBarcodeScanner
{
    /// <inheritdoc/>
    public async Task<string> ScanBarcodeAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new BarcodeScanException("Failed to find top UIViewController.", new NotSupportedException($"{nameof(topViewController)} can not be null."));

        if (!DataScannerViewController.IsSupported)
        {
            throw new BarcodeScanException("Data scanner not supported.", new DataScannerUnsupportedException(string.Join(", ", DataScannerViewController.ScanningUnavailable)));
        }

        if (!DataScannerViewController.IsAvailable)
        {
            throw new BarcodeScanException("Data scanner not available.", new DataScannerUnavailableException(string.Join(", ", DataScannerViewController.ScanningUnavailable)));
        }

        TaskCompletionSource<string> taskSource = new();

        using RecognizedDataType barcodeType = RecognizedDataType.Barcode(options.Formats.ToBarcodeFormats().ToArray());
        using DataScannerViewController scanner = new([barcodeType], isGuidanceEnabled: false, isHighlightingEnabled: false);
        scanner.Delegate = new BarcodeScannerDelegate(scanner, barcode => taskSource.TrySetResult(barcode));

        UIView overlayView = scanner.AddBarcodeRegionOfInterestOverlay();

        await topViewController.PresentViewControllerAsync(scanner.ScannerViewController, true).ConfigureAwait(true);

        scanner.StartScanning(out DataScannerStartException? error);
        if (error is not null)
        {
            _ = taskSource.TrySetException(new BarcodeScanException("Could not start scanner.", error));
        }

        string barcode = await taskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        await scanner.ScannerViewController.DismissViewControllerAsync(true).ConfigureAwait(true);

        overlayView.Dispose();

        return barcode;
    }
}