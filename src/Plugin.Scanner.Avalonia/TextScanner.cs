using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Avalonia;

public static class TextScanner
{
    private static ITextScanner? _textScannerImplementation;

#if !IOS && !ANDROID
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.Core.Scanners.TextScanner();
#endif

#if IOS
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.iOS.Scanners.TextScanner();
#endif

#if ANDROID
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.Android.Scanners.TextScanner(new Android.CurrentActivity());
#endif
}