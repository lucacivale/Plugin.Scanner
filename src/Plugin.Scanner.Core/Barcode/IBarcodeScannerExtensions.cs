#pragma warning disable IDE0005
using System.Threading;
using System.Threading.Tasks;
#pragma warning restore IDE0005
using Plugin.Scanner.Core.Exceptions;

namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// <see cref="IBarcodeScanner"/> extension methods.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IBarcodeScannerExtensions
{
    /// <summary>
    /// Scan a single barcode without a cancellation token.
    /// </summary>
    /// <param name="scanner">Scanner.</param>
    /// <param name="options">Configure barcode scan.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="BarcodeScanException">Throws an <see cref="BarcodeScanException"/> if something went wrong. For more details check platform implementations.</exception>
    public static Task<string> ScanBarcodeAsync(this IBarcodeScanner scanner, IBarcodeScanOptions options)
    {
        return scanner.ScanBarcodeAsync(options, CancellationToken.None);
    }
}