namespace Plugin.Scanner.Core.Scanners;

internal sealed class DocumentScanner : IDocumentScanner
{
    public Task<IDocument> ScanAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IDocument>(new Models.Document([]));
    }
}
