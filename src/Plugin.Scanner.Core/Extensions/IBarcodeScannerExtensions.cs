using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IBarcodeScanner"/>.
/// </summary>
public static class IBarcodeScannerExtensions
{
    /// <summary>
    /// Asynchronously scans for a barcode using the specified options.
    /// </summary>
    /// <param name="scanner">The barcode scanner instance.</param>
    /// <param name="options">The scan options specifying barcode formats and behavior.</param>
    /// <returns>A task that represents the asynchronous scan operation. The task result contains the scan result.</returns>
    public static Task<IScanResult> ScanAsync(this IBarcodeScanner scanner, IBarcodeScanOptions options)
    {
        return scanner.ScanAsync(options, CancellationToken.None);
    }
}