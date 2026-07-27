using Microsoft.Maui.Platform;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    public partial async Task AttachBarcodeScanner(Page page, IBarcodeScanOptions options)
    {
        if (await Attach(page, options).ConfigureAwait(true) == false)
        {
            return;
        }

        _barcodeScannerPopup.Attach(page.ToPlatform(page.Handler!.MauiContext!), options);
        _scannerAttached = true;
    }
}
