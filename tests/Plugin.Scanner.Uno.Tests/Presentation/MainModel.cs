using System.Diagnostics;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;

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
            string barcode = (await _barcodeScanner.ScanAsync(new BarcodeScanOptions { Formats = BarcodeFormat.All }).ConfigureAwait(false)).RawValue;
            await Barcode.SetAsync(barcode);
        }
        catch (BarcodeScanException exception)
        {
            Debug.WriteLine(exception);

            await Barcode.SetAsync("Something went wrong.");
        }
    }
}
