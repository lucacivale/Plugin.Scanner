using Plugin.Scanner.Core.Scanners.Popups;

namespace Plugin.Scanner.Avalonia;

public static class TextScannerPopup
{
    private static ITextScannerPopup? _textScannerPopupImplementation;

#if !IOS && !ANDROID
    public static ITextScannerPopup Default => _textScannerPopupImplementation ??= new Plugin.Scanner.Core.Scanners.Popups.TextScannerPopup();
#endif

#if IOS
    public static ITextScannerPopup Default => _textScannerPopupImplementation ??= new iOS.Scanners.Popups.TextScannerPopup();
#endif

#if ANDROID
    public static ITextScannerPopup Default => _textScannerPopupImplementation ??= new Scanner.Android.Scanners.Popups.TextScannerPopup(new Android.CurrentActivity());
#endif
}
