using System.Diagnostics;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Extensions;
using Plugin.Scanner.Core.Scanners;
using Plugin.Scanner.Models;
using Plugin.Scanner.Options;
using Plugin.Scanner.Overlays.Barcode;

namespace Plugin.Scanner.Uno.Tests.Presentation;

public partial record MainModel
{
    private readonly IBarcodeScanner _barcodeScanner;

    public MainModel(IBarcodeScanner barcodeScanner)
    {
        _barcodeScanner = barcodeScanner;
    }

    public IState<string> Barcode => State<string>.Value(this, () => string.Empty);
    
    public async Task ScanBarcode()
    {
        try
        {
            BarcodeScanOptions options = new()
            {
                Formats = BarcodeFormat.All,
                IsHighlightingEnabled = true,
                RegionOfInterest = new CenteredRegionOfInterest(250, 200),
                Overlay = new DefaultBarcodeScannerOverlay(),
            };

            string barcode = (await _barcodeScanner.ScanAsync(options).ConfigureAwait(false)).Value;

            await Barcode.SetAsync(barcode);
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            await Barcode.SetAsync("Something went wrong.");
        }
    }
}
