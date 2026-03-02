namespace Plugin.Scanner.Core.Scanners;

internal sealed class DocumentScanner : IDocumentScanner
{
    public Task<IReadOnlyList<IDocument>> ScanAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<IDocument>>([]);
    }
}
