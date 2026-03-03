using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners;

/// <summary>
/// Provides a default implementation of <see cref="ITextScanner"/> that returns an empty result.
/// </summary>
/// <remarks>
/// This is a placeholder implementation intended to be replaced by platform-specific implementations.
/// </remarks>
internal sealed class TextScanner : ITextScanner
{
    /// <summary>
    /// Asynchronously scans and recognizes text using the device camera.
    /// </summary>
    /// <param name="options">The options specifying text recognition settings.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// This default implementation returns an empty result.
    /// </returns>
    public Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken)
    {
        return Task.FromResult<IScanResult>(new ScanResult(string.Empty));
    }
}