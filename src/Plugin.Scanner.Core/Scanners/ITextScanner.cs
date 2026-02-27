using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners;

public interface ITextScanner
{
    Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken);
}