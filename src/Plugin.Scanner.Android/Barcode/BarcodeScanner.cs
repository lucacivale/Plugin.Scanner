using Plugin.Scanner.Core.Barcode;
using Xamarin.Google.MLKit.Vision.CodeScanner;
using Task = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android.Barcode;

/// <summary>
/// Barcode scanner.
/// </summary>
public sealed class BarcodeScanner : IBarcodeScanner
{
    /// <inheritdoc/>
    public async Task<string> ScanBarcodeAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        using GmsBarcodeScannerOptions.Builder builder = new();

        IEnumerable<int> formats = options.Formats.ToBarcodeFormats().ToArray();
        _ = builder.SetBarcodeFormats(formats.First(), formats.Skip(1).ToArray());

        using GmsBarcodeScannerOptions gmsOptions = builder.Build();
        using IGmsBarcodeScanner barcodeScanner = GmsBarcodeScanning.GetClient(Application.Context, gmsOptions);

        TaskCompletionSource<string> taskCompletionSource = new();
        using BarcodeCompleteListener barcodeCompleteListener = new(taskCompletionSource);

        using Task task = barcodeScanner
            .StartScan()
            .AddOnCompleteListener(barcodeCompleteListener);

        return await taskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
    }
}