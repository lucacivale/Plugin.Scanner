using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Avalonia;

/// <summary>
/// Provides access to the default implementation of the document scanning functionality.
/// </summary>
public static class DocumentScanner
{
    private static IDocumentScanner? _documentScannerImplementation;

#if !IOS && !ANDROID
    /// <summary>
    /// Gets the default implementation of the <see cref="IDocumentScanner"/> interface.
    /// </summary>
    /// <remarks>
    /// This property provides a singleton instance of the document scanner implementation.
    /// If no implementation is set, a default instance of <see cref="Plugin.Scanner.Core.Scanners.DocumentScanner"/> is created and returned.
    /// </remarks>
    /// <value>
    /// An <see cref="IDocumentScanner"/> instance representing the default implementation of the document scanning functionality.
    /// </value>
    public static IDocumentScanner Default => _documentScannerImplementation ??= new Plugin.Scanner.Core.Scanners.DocumentScanner();
#endif

#if IOS
    /// <summary>
    /// Gets the default implementation of the <see cref="IDocumentScanner"/> interface.
    /// If no implementation is explicitly provided, the property initializes and returns
    /// a platform-specific instance of an <see cref="IDocumentScanner"/>.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IDocumentScanner"/> for document scanning functionality.
    /// </returns>
    public static IDocumentScanner Default => _documentScannerImplementation ??= new iOS.Scanners.DocumentScanner();
#endif

#if ANDROID
    /// <summary>
    /// Gets the default implementation of the <see cref="IDocumentScanner"/> interface.
    /// When accessed, it initializes the default document scanner implementation, if not already set,
    /// and returns the instance of the scanner.
    /// </summary>
    /// <remarks>
    /// The default implementation uses platform-specific details to provide document scanning functionality.
    /// Initialization occurs lazily, ensuring the instance is only created when accessed.
    /// </remarks>
    public static IDocumentScanner Default => _documentScannerImplementation ??= new Scanner.Android.Scanners.DocumentScanner(new Android.CurrentActivity());
#endif
}
