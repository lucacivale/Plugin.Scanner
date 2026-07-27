using Plugin.Scanner.Core.Scanners.Popups;

namespace Plugin.Scanner.Avalonia;

public static class BarcodeScannerPopup
{
    private static IBarcodeScannerPopup? _barcodeScannerPopupImplementation;

#if !IOS && !ANDROID
    public static IBarcodeScannerPopup Default => _barcodeScannerPopupImplementation ??= new Plugin.Scanner.Core.Scanners.Popups.BarcodeScannerPopup();
#endif

#if IOS
    public static IBarcodeScannerPopup Default => _barcodeScannerPopupImplementation ??= new iOS.Scanners.Popups.BarcodeScannerPopup();
#endif

#if ANDROID
    public static IBarcodeScannerPopup Default => _barcodeScannerPopupImplementation ??= new Scanner.Android.Scanners.Popups.BarcodeScannerPopup(new Android.CurrentActivity());
#endif
}
