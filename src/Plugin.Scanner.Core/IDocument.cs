namespace Plugin.Scanner.Core;

/// <summary>
/// Represents a scanned document containing one or more pages.
/// </summary>
public interface IDocument
{
    /// <summary>
    /// Gets the collection of pages in the document.
    /// </summary>
    IReadOnlyList<IDocumentPage> Pages { get; }
}