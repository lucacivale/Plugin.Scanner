using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;

namespace Plugin.Scanner.Core.Extensions;

public static class ITextScannerExtensions
{
    public static Task<IScanResult> ScanAsync(this ITextScanner scanner, ITextScanOptions options)
    {
        return scanner.ScanAsync(options, CancellationToken.None);
    }
}