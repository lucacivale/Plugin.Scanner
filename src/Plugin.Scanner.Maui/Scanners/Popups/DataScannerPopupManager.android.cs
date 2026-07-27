using Microsoft.Maui.Platform;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    private partial void AttachBarcodeScanner(Page page, IMauiContext context, IBarcodeScanOptions options)
    {
        _barcodeScannerPopup.Attach(page.ToPlatform(context), options);
    }
}
