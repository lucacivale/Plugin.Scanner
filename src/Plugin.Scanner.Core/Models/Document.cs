
namespace Plugin.Scanner.Core.Models;

internal sealed class Document : IDocument
{
    public Document(IEnumerable<IDocumentPage> pages)
    {
        Pages = pages.ToList();
    }

    public IReadOnlyList<IDocumentPage> Pages { get; }
}