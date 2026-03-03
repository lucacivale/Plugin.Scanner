namespace Plugin.Scanner.Core;

/// <summary>
/// Represents a single page of a scanned document.
/// </summary>
public interface IDocumentPage
{
    /// <summary>
    /// Retrieves the raw byte data of the scanned document page.
    /// </summary>
    /// <returns>A byte array containing the data of the document page.</returns>
    byte[] GetRawData();
}
