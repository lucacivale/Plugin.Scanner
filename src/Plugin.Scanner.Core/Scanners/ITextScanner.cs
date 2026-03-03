using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners;

/// <summary>
/// Defines the contract for text recognition (OCR) functionality.
/// </summary>
public interface ITextScanner
{
    /// <summary>
    /// Asynchronously scans and recognizes text using the device camera.
    /// </summary>
    /// <param name="options">The options specifying text recognition settings.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the recognized text.</returns>
    Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken);
}