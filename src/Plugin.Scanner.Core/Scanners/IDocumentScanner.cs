namespace Plugin.Scanner.Core.Scanners;

public interface IDocumentScanner
{
    Task<IDocument> ScanAsync(CancellationToken cancellationToken);
}
