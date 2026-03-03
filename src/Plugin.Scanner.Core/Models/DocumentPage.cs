namespace Plugin.Scanner.Core.Models;

/// <summary>
/// Represents a single page of a scanned document.
/// </summary>
internal sealed class DocumentPage : IDocumentPage
{
    private readonly byte[] _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentPage"/> class.
    /// </summary>
    /// <param name="data">The raw byte data of the page.</param>
    public DocumentPage(byte[] data)
    {
        _data = data;
    }

    /// <summary>
    /// Retrieves the raw byte data of the scanned document page.
    /// </summary>
    /// <returns>A byte array containing the data of the document page.</returns>
    public byte[] GetRawData()
    {
        return _data;
    }
}
