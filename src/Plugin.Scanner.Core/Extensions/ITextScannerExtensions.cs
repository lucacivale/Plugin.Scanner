using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ITextScanner"/> to simplify common scanning operations.
/// </summary>
public static class ITextScannerExtensions
{
    /// <summary>
    /// Asynchronously scans for text using the device camera without a cancellation token.
    /// </summary>
    /// <param name="scanner">The <see cref="ITextScanner"/> instance to use for scanning.</param>
    /// <param name="options">The <see cref="ITextScanOptions"/> specifying text recognition settings.</param>
    /// <returns>A <see cref="Task{IScanResult}"/> that represents the asynchronous scan operation.</returns>
    public static Task<IScanResult> ScanAsync(this ITextScanner scanner, ITextScanOptions options)
    {
        return scanner.ScanAsync(options, CancellationToken.None);
    }
}