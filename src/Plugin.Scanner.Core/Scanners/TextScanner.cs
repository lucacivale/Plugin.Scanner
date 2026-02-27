using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners;

internal sealed class TextScanner : ITextScanner
{
    public Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken)
    {
        return Task.FromResult<IScanResult>(new ScanResult(string.Empty));
    }
}