#pragma warning disable IDE0005
using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning restore IDE0005

namespace Plugin.Scanner.Core.Barcode;

/// <inheritdoc />
public class BarcodeScanner : IBarcodeScanner
{
    /// <inheritdoc/>
    public Task<string> ScanBarcodeAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}