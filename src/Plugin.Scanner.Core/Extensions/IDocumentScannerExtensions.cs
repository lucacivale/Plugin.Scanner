using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Core.Extensions;

public static class IDocumentScannerExtensions
{
    public static Task<IDocument> ScanAsync(this IDocumentScanner scanner)
    {
        return scanner.ScanAsync(CancellationToken.None);
    }
}
