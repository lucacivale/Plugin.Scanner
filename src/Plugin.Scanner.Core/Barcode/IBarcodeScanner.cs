namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Defines the contract for barcode scanning functionality across different platforms.
/// </summary>
/// <remarks>
/// <para>
/// This interface is implemented by platform-specific barcode scanner classes that provide
/// access to the device camera for scanning various barcode formats.
/// </para>
/// <para>
/// Platform-specific implementations:
/// <list type="bullet">
/// <item><description>iOS: <c>Plugin.Scanner.iOS.Barcode.BarcodeScanner</c></description></item>
/// <item><description>Android: <c>Plugin.Scanner.Android.Barcode.BarcodeScanner</c></description></item>
/// </list>
/// </para>
/// </remarks>
public interface IBarcodeScanner
{
    /// <summary>
    /// Asynchronously scans for a barcode using the device camera.
    /// </summary>
    /// <param name="options">The <see cref="IBarcodeScanOptions"/> specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the scan operation.</param>
    /// <returns>
    /// A <see cref="Task{IBarcode}"/> that represents the asynchronous scan operation.
    /// The task result contains the scanned barcode with its decoded value.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method opens the device camera interface and waits for the user to scan a barcode
    /// or cancel the operation. The scanning interface is dismissed automatically when a barcode
    /// is successfully scanned or when the operation is canceled.
    /// </para>
    /// <para>
    /// The method can be canceled using the provided <paramref name="cancellationToken"/> or
    /// through user interaction with the scanning interface (e.g., pressing a cancel button).
    /// </para>
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the scan operation is canceled via the <paramref name="cancellationToken"/>
    /// or by the user.
    /// </exception>
    /// <example>
    /// Basic usage:
    /// <code>
    /// var options = new BarcodeScanOptions
    /// {
    ///     Formats = BarcodeFormat.QR | BarcodeFormat.Ean13
    /// };
    ///
    /// using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
    /// var barcode = await scanner.ScanAsync(options, cts.Token);
    /// Console.WriteLine($"Scanned: {barcode.RawValue}");
    /// </code>
    /// </example>
    Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken);
}