namespace Plugin.Scanner.Core.Scanners;

/// <summary>
/// Provides a default implementation of <see cref="IDocumentScanner"/> that returns an empty document.
/// </summary>
/// <remarks>
/// This is a placeholder implementation intended to be replaced by platform-specific implementations.
/// </remarks>
internal sealed class DocumentScanner : IDocumentScanner
{
    /// <summary>
    /// Asynchronously scans a document using the device camera.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// This default implementation returns an empty document.
    /// </returns>
    public Task<IDocument> ScanAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IDocument>(new Models.Document([]));
    }
}
