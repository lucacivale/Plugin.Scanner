namespace Plugin.Scanner.Core.Models;

/// <summary>
/// Represents a scanned document containing one or more pages.
/// </summary>
internal sealed class Document : IDocument
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Document"/> class.
    /// </summary>
    /// <param name="pages">The collection of pages in the document.</param>
    public Document(IEnumerable<IDocumentPage> pages)
    {
        Pages = pages.ToList();
    }

    /// <summary>
    /// Gets the read-only list of pages in the document.
    /// </summary>
    public IReadOnlyList<IDocumentPage> Pages { get; }
}