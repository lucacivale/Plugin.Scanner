using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IDocumentScanner"/> to simplify common scanning operations.
/// </summary>
public static class IDocumentScannerExtensions
{
    /// <summary>
    /// Asynchronously scans a document using the device camera without a cancellation token.
    /// </summary>
    /// <param name="scanner">The <see cref="IDocumentScanner"/> instance to use for scanning.</param>
    /// <returns>A <see cref="Task{IDocument}"/> that represents the asynchronous scan operation.</returns>
    public static Task<IDocument> ScanAsync(this IDocumentScanner scanner)
    {
        return scanner.ScanAsync(CancellationToken.None);
    }
}
