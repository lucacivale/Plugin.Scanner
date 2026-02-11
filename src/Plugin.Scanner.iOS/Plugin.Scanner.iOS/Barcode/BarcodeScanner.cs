using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Barcode;

/// <summary>
/// Provides barcode scanning functionality using the device camera.
/// </summary>
public sealed class BarcodeScanner : IBarcodeScanner
{
    /// <summary>
    /// Asynchronously scans for a barcode using the device camera.
    /// </summary>
    /// <param name="options">The <see cref="IBarcodeScanOptions"/> specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> token to cancel the scan operation.</param>
    /// <returns>A <see cref="Task{IBarcode}"/> that represents the asynchronous operation. The task result contains the scanned barcode.</returns>
    /// <exception cref="BarcodeScanException">
    /// Thrown when a scanner-related error occurs, including
    /// camera configuration issues, invalid event types, scanner start failures,
    /// torch mode problems, scanner availability issues, or view controller errors.
    /// </exception>
    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        try
        {
            using RecognizedDataType barcodeType =
                RecognizedDataType.Barcode(options.Formats.ToBarcodeFormats().ToArray());
            using SingleBarcodeScannerViewController scanner = new([barcodeType]);

            return await scanner.ScanAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception e)
            when (e is DataScannerCameraConfigurationLockException
                      or DataScannerEventSenderInvalidTypeException
                      or DataScannerStartException
                      or DataScannerTorchModeUnsupportedException
                      or DataScannerTorchUnavailableException
                      or DataScannerUnavailableException
                      or DataScannerUnsupportedException
                      or DataScannerViewControllerNotFoundException
                      or DataScannerViewNullReferenceException)
        {
            throw new BarcodeScanException(e.Message, e);
        }
    }
}