using Plugin.Scanner.Core.Options;
using AvaloniaControl = Avalonia.Controls.Control;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    private partial void AttachBarcodeScanner(AvaloniaControl control, IBarcodeScanOptions options)
    {
        _barcodeScannerPopup.Attach(page.ToPlatform(context), options);
    }

    private partial void AttachTextScanner(AvaloniaControl control, ITextScanOptions options)
    {
        _textScannerPopup.Attach(page.ToPlatform(context), options);
    }
}
