using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Avalonia;

public static class DocumentScanner
{
    private static IDocumentScanner? _documentScannerImplementation;

#if !IOS && !ANDROID
    public static IDocumentScanner Default => _documentScannerImplementation ??= new Plugin.Scanner.Core.Scanners.DocumentScanner();
#endif

#if IOS
    public static IDocumentScanner Default => _documentScannerImplementation ??= new iOS.Scanners.DocumentScanner();
#endif

#if ANDROID
    public static IDocumentScanner Default => _documentScannerImplementation ??= new Scanner.Android.Scanners.DocumentScanner(new Android.CurrentActivity());
#endif
}
