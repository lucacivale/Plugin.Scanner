using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    public partial Task AttachBarcodeScanner(Page page, IBarcodeScanOptions options)
    {
        return Task.CompletedTask;
    }
}
