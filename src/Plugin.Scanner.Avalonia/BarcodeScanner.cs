using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner.Avalonia;

public static class BarcodeScanner
{
    private static IBarcodeScanner? _barcodeScannerImplementation;

#if !IOS && !ANDROID
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Plugin.Scanner.Core.Barcode.BarcodeScanner();

#endif

#if IOS
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new iOS.Barcode.BarcodeScanner();
#endif

#if ANDROID
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Plugin.Scanner.Android.Barcode.BarcodeScanner(new Plugin.Scanner.Avalonia.Android.CurrentActivity());
#endif
}
