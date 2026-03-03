using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Avalonia;

/// <summary>
/// A static class that provides access to the default implementation of the barcode scanner.
/// </summary>
public static class BarcodeScanner
{
    private static IBarcodeScanner? _barcodeScannerImplementation;

#if !IOS && !ANDROID
    /// <summary>
    /// Gets the default implementation of the <see cref="IBarcodeScanner"/> interface.
    /// This property provides a singleton instance of the barcode scanner,
    /// which can be used to perform barcode scanning operations.
    /// </summary>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Plugin.Scanner.Core.Scanners.BarcodeScanner();
#endif

#if IOS
    /// <summary>
    /// Gets the default implementation of the <see cref="IBarcodeScanner"/> interface.
    /// </summary>
    /// <remarks>
    /// This property initializes an instance of the default barcode scanner implementation
    /// for the platform, if it is not already set.
    /// </remarks>
    /// <value>
    /// An instance of <see cref="IBarcodeScanner"/> that represents the default barcode scanner implementation.
    /// </value>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new iOS.Scanners.BarcodeScanner();
#endif

#if ANDROID
    /// <summary>
    /// Gets the default instance of <see cref="IBarcodeScanner"/>.
    /// This property provides access to the default implementation of the barcode scanner,
    /// initializing it if necessary. The default implementation is platform-specific and
    /// may vary depending on the environment (e.g., Android, iOS) in which the application is run.
    /// </summary>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Scanner.Android.Scanners.BarcodeScanner(new Android.CurrentActivity());
#endif
}
