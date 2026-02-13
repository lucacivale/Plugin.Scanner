using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner.Avalonia;

/// <summary>
/// Provides cross-platform access to barcode scanning functionality.
/// </summary>
/// <remarks>
/// This class automatically selects the appropriate platform-specific implementation
/// based on the target platform (iOS, Android, or Core).
/// </remarks>
/// <example>
/// <code>
/// // Basic usage
/// var result = await BarcodeScanner.Default.ScanAsync();
/// Console.WriteLine($"Scanned: {result.RawValue}");
/// </code>
/// </example>
public static class BarcodeScanner
{
    private static IBarcodeScanner? _barcodeScannerImplementation;

#if !IOS && !ANDROID
    /// <summary>
    /// Gets the default platform-specific barcode scanner implementation.
    /// </summary>
    /// <value>
    /// An <see cref="IBarcodeScanner"/> instance that provides barcode scanning capabilities.
    /// The implementation is lazily initialized on first access and reused for further calls.
    /// </value>
    /// <remarks>
    /// <para>Platform-specific behavior:</para>
    /// <list type="bullet">
    /// <item><description>Other: Uses the core scanner implementation</description></item>
    /// </list>
    /// </remarks>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Plugin.Scanner.Core.Barcode.BarcodeScanner();
#endif

#if IOS
    /// <summary>
    /// Gets the default platform-specific barcode scanner implementation.
    /// </summary>
    /// <value>
    /// An <see cref="IBarcodeScanner"/> instance that provides barcode scanning capabilities.
    /// The implementation is lazily initialized on first access and reused for further calls.
    /// </value>
    /// <remarks>
    /// <para>Platform-specific behavior:</para>
    /// <list type="bullet">
    /// <item><description>iOS: Uses native DataScannerViewController with camera access</description></item>
    /// </list>
    /// </remarks>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new iOS.Barcode.BarcodeScanner();
#endif

#if ANDROID
    /// <summary>
    /// Gets the default platform-specific barcode scanner implementation.
    /// </summary>
    /// <value>
    /// An <see cref="IBarcodeScanner"/> instance that provides barcode scanning capabilities.
    /// The implementation is lazily initialized on first access and reused for further calls.
    /// </value>
    /// <remarks>
    /// <para>Platform-specific behavior:</para>
    /// <list type="bullet">
    /// <item><description>Android: Uses Googles ML-Kit camera-based scanner with activity context</description></item>
    /// </list>
    /// </remarks>
    public static IBarcodeScanner Default => _barcodeScannerImplementation ??= new Scanner.Android.Barcode.BarcodeScanner(new Android.CurrentActivity());
#endif
}
