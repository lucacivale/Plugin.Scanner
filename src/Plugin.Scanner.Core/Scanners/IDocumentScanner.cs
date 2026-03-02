namespace Plugin.Scanner.Core.Scanners;

public interface IDocumentScanner
{
    Task<IReadOnlyList<IDocument>> ScanAsync(CancellationToken cancellationToken);
}
