using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner.Android.Barcode;

internal sealed class BarcodeScanner : IBarcodeScanner
{
    private readonly ICurrentActivity _currentActivity;

    public BarcodeScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        IBarcode barcode;

        using (SingleBarcodeScannerDialog scannerDialog = new(_currentActivity.GetActivity(), _Microsoft.Android.Resource.Designer.Resource.Style.Plugin_Scanner_SingleBarcodeScanner))
        {
            scannerDialog.Show();
            barcode = await scannerDialog.ScanAsync(options, cancellationToken).ConfigureAwait(true);

        }
        return barcode ?? new Core.Barcode.Barcode(string.Empty);
    }
}