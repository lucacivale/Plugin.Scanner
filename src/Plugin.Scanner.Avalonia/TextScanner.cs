using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Avalonia;

/// <summary>
/// Provides functionality for text scanning operations.
/// </summary>
/// <remarks>
/// This class is responsible for managing the default implementation of the text scanning functionality.
/// It serves as an entry point for accessing the <see cref="ITextScanner"/> implementation.
/// </remarks>
public static class TextScanner
{
    private static ITextScanner? _textScannerImplementation;

#if !IOS && !ANDROID
    /// <summary>
    /// Gets the default implementation of <see cref="ITextScanner"/>.
    /// </summary>
    /// <remarks>
    /// This property lazily initializes and provides access to a singleton instance of <see cref="ITextScanner"/>.
    /// If no implementation is already set, it defaults to an internal instance of <see cref="Plugin.Scanner.Core.Scanners.TextScanner"/>.
    /// </remarks>
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.Core.Scanners.TextScanner();
#endif

#if IOS
    /// <summary>
    /// Gets the default implementation of the <see cref="ITextScanner"/> interface.
    /// </summary>
    /// <remarks>
    /// If no implementation has been previously set, a new instance of the iOS-specific <see cref="Plugin.Scanner.iOS.Scanners.TextScanner"/>
    /// will be created and returned. This property provides a centralized way to access the default text scanner functionality.
    /// </remarks>
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.iOS.Scanners.TextScanner();
#endif

#if ANDROID
    /// <summary>
    /// Gets the default implementation of the <see cref="ITextScanner"/> interface.
    /// </summary>
    /// <remarks>
    /// This property provides access to a platform-specific instance of <see cref="ITextScanner"/>.
    /// If no instance is currently set, it initializes and returns a new platform-specific implementation.
    /// </remarks>
    /// <value>
    /// The default <see cref="ITextScanner"/> implementation for text recognition operations.
    /// </value>
    public static ITextScanner Default => _textScannerImplementation ??= new Plugin.Scanner.Android.Scanners.TextScanner(new Android.CurrentActivity());
#endif
}