#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
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