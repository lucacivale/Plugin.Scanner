#pragma warning disable IDE0005
using System.Threading;
using System.Threading.Tasks;
#pragma warning restore IDE0005
using Plugin.Scanner.Core.Exceptions;

namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Barcode scanner cross-platform interface.
/// </summary>
public interface IBarcodeScanner
{
    /// <summary>
    /// Scan a single barcode.
    /// </summary>
    /// <param name="options">Configure barcode scan.</param>
    /// <param name="cancellationToken">Cancel task.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="BarcodeScanException">Throws an <see cref="BarcodeScanException"/> if something went wrong. For more details check platform implementations.</exception>
    public Task<string> ScanBarcodeAsync(IBarcodeScanOptions options, CancellationToken cancellationToken);
}