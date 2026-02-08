namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Provides a default implementation of <see cref="IBarcodeScanner"/> that returns an empty barcode.
/// </summary>
/// <remarks>
/// <para>
/// This is a placeholder implementation intended to be replaced by platform-specific implementations.
/// </para>
/// <para>
/// For actual barcode scanning functionality, use the platform-specific implementations:
/// <list type="bullet">
/// <item><description>iOS: <c>Plugin.Scanner.iOS.Barcode.BarcodeScanner</c></description></item>
/// <item><description>Android: <c>Plugin.Scanner.Android.Barcode.BarcodeScanner</c></description></item>
/// </list>
/// </para>
/// </remarks>
public class BarcodeScanner : IBarcodeScanner
{
    /// <summary>
    /// Asynchronously scans for a barcode using the device camera.
    /// </summary>
    /// <param name="options">The <see cref="IBarcodeScanOptions"/> specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the scan operation.</param>
    /// <returns>
    /// A <see cref="Task{IBarcode}"/> that represents the asynchronous operation.
    /// This default implementation returns a completed task with an empty barcode.
    /// </returns>
    /// <remarks>
    /// This is a placeholder implementation that always returns an empty barcode.
    /// Platform-specific implementations should override this behavior to provide actual scanning functionality.
    /// </remarks>
    public Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        return Task.FromResult<IBarcode>(new Barcode(string.Empty));
    }
}