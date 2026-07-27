using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups;

public interface IDataScannerPopupManager
{
    Task AttachBarcodeScanner(Page page, IBarcodeScanOptions options);

    void Detach();
}
