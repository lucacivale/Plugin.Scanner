using CoreFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.iOS.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Plugin.Scanner.iOS.Scanners;

internal abstract class Scanner<TOptions>
    where TOptions : IScanOptions
{
    /// <summary>
    /// Scans for barcodes or texts using the device camera with the specified options.
    /// </summary>
    /// <param name="options">The scan configuration options.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous scan operation, containing the scan result.</returns>
    /// <exception cref="ScanException">Thrown when the scan operation fails.</exception>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IScanResult> ScanAsync(TOptions options, CancellationToken cancellationToken)
    {
        TaskCompletionSource<IScanResult> scanCompleteTaskSource = new();

        DispatchQueue.MainQueue.DispatchAsync(async () =>
        {
            try
            {
                using DataScannerViewController scanner = CreateViewController(options);

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

    protected abstract DataScannerViewController CreateViewController(TOptions options);
}
