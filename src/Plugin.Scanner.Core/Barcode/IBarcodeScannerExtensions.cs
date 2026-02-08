namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Provides extension methods for <see cref="IBarcodeScanner"/> to simplify common scanning operations.
/// </summary>
public static class IBarcodeScannerExtensions
{
    /// <summary>
    /// Asynchronously scans for a barcode using the device camera without a cancellation token.
    /// </summary>
    /// <param name="scanner">The <see cref="IBarcodeScanner"/> instance to use for scanning.</param>
    /// <param name="options">The <see cref="IBarcodeScanOptions"/> specifying which barcode formats to recognize.</param>
    /// <returns>
    /// A <see cref="Task{IBarcode}"/> that represents the asynchronous scan operation.
    /// The task result contains the scanned barcode with its decoded value.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that calls <see cref="IBarcodeScanner.ScanAsync"/> with
    /// <see cref="CancellationToken.None"/>, meaning the scan operation can only be canceled
    /// through user interaction (e.g., pressing a cancel button in the scanning interface).
    /// </para>
    /// <para>
    /// For operations that require programmatic cancellation, use <see cref="IBarcodeScanner.ScanAsync"/>
    /// directly with a <see cref="CancellationToken"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// Simple barcode scanning:
    /// <code>
    /// var scanner = new BarcodeScanner();
    /// var options = new BarcodeScanOptions
    /// {
    ///     Formats = BarcodeFormat.All
    /// };
    ///
    /// var barcode = await scanner.ScanBarcodeAsync(options);
    /// Console.WriteLine($"Scanned: {barcode.RawValue}");
    /// </code>
    /// </example>
    public static Task<IBarcode> ScanBarcodeAsync(this IBarcodeScanner scanner, IBarcodeScanOptions options)
    {
        return scanner.ScanAsync(options, CancellationToken.None);
    }
}