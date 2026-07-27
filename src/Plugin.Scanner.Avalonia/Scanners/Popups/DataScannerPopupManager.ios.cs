using Avalonia.Controls;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    private partial void AttachBarcodeScanner(Control control, IBarcodeScanOptions options)
    {
        _barcodeScannerPopup.Attach(page.ToUIViewController(context), options);
    }

    private partial void AttachTextScanner(Control control, ITextScanOptions options)
    {
        _textScannerPopup.Attach(page.ToUIViewController(context), options);
    }
}
