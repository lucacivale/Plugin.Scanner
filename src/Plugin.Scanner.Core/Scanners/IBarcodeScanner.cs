using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners;

/// <summary>
/// Defines the contract for barcode scanning functionality.
/// </summary>
public interface IBarcodeScanner
{
    /// <summary>
    /// Asynchronously scans for a barcode using the device camera.
    /// </summary>
    /// <param name="options">The options specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the scanned barcode.</returns>
    Task<IScanResult> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken);
}