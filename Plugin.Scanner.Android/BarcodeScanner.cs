using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Xamarin.Google.MLKit.Vision.CodeScanner;
using Task = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android;

/// <summary>
/// Barcode scanner.
/// </summary>
public static class BarcodeScanner
{
    /// <summary>
    /// Open a modal dialog to scan a single barcode. After barcode detection the dialog is closed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task<string> ScanBarcodeAsync()
    {
        return ScanBarcodeAsync(CancellationToken.None);
    }

    /// <summary>
    /// Open a modal dialog to scan a single barcode. After barcode detection the dialog is closed.
    /// </summary>
    /// <param name="cancellationToken">Cancel task.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<string> ScanBarcodeAsync(CancellationToken cancellationToken)
    {
        using GmsBarcodeScannerOptions.Builder builder = new();
        builder.SetBarcodeFormats(Barcode.FormatAllFormats);

        using GmsBarcodeScannerOptions options = builder.Build();
        using IGmsBarcodeScanner barcodeScanner = GmsBarcodeScanning.GetClient(Application.Context, options);

        TaskCompletionSource<string> taskCompletionSource = new();
        using BarcodeCompleteListener barcodeCompleteListener = new(taskCompletionSource);

        using Task task = barcodeScanner
            .StartScan()
            .AddOnCompleteListener(barcodeCompleteListener);

        return await taskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
    }
}