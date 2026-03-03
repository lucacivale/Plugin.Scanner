using System.Diagnostics.CodeAnalysis;
using CoreFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;

namespace Plugin.Scanner.iOS.Scanners;

/// <summary>
/// Provides text scanning functionality for iOS using the DataScanner framework.
/// </summary>
internal sealed class TextScanner : ITextScanner
{
    /// <summary>
    /// Scans for text using the device camera with the specified options.
    /// </summary>
    /// <param name="options">The text scan configuration options.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous scan operation, containing the scan result.</returns>
    /// <exception cref="ScanException">Thrown when the scan operation fails.</exception>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken)
    {
        TaskCompletionSource<IScanResult> scanCompleteTaskSource = new();

        DispatchQueue.MainQueue.DispatchAsync(async () =>
        {
            try
            {
                using RecognizedDataType barcodeType = RecognizedDataType.Text(Binding.DataScannerViewController.SupportedTextRecognitionLanguages, TextContentType.Default);
                using DataScannerViewController scanner = new(
                    [barcodeType],
                    recognizesMultipleItems: true,
                    isHighlightingEnabled: options.IsHighlightingEnabled,
                    isPinchToZoomEnabled: options.IsPinchToZoomEnabled,
                    regionOfInterest: options.RegionOfInterest,
                    overlay: options.Overlay);

                scanCompleteTaskSource.TrySetResult(await scanner.ScanAsync(cancellationToken).ConfigureAwait(true));
            }
            catch (Exception e)
            {
                scanCompleteTaskSource.TrySetException(e);
            }
        });

        try
        {
            return await scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception e)
            when (e is DataScannerCameraConfigurationLockException
                      or DataScannerEventSenderInvalidTypeException
                      or DataScannerStartException
                      or DataScannerTorchModeUnsupportedException
                      or DataScannerTorchUnavailableException
                      or DataScannerUnavailableException
                      or DataScannerUnsupportedException
                      or DataScannerViewNullReferenceException)
        {
            throw new ScanException(e.Message, e);
        }
    }
}