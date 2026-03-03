namespace Plugin.Scanner.Core.Scanners;

/// <summary>
/// Defines the contract for document scanning functionality.
/// </summary>
public interface IDocumentScanner
{
    /// <summary>
    /// Asynchronously scans a document using the device camera.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the scanned document.</returns>
    Task<IDocument> ScanAsync(CancellationToken cancellationToken);
}
