using System.Diagnostics;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Models;
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

            string barcode = (await _barcodeScanner.ScanAsync(options).ConfigureAwait(false)).RawValue;

            await Barcode.SetAsync(barcode);
        }
        catch (BarcodeScanException exception)
        {
            Debug.WriteLine(exception);

            await Barcode.SetAsync("Something went wrong.");
        }
    }
}
