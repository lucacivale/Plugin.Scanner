using CoreFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Scanners;
using System.Diagnostics.CodeAnalysis;
using VisionKit;

namespace Plugin.Scanner.iOS.Scanners;

/// <summary>
/// Provides document scanning functionality for iOS using the VisionKit framework.
/// </summary>
internal sealed class DocumentScanner : VNDocumentCameraViewControllerDelegate, IDocumentScanner
{
    private TaskCompletionSource<IDocument>? _scanCompleteTaskSource;

    /// <summary>
    /// Scans a document using the device camera.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task that represents the asynchronous scan operation, containing the scanned document with its pages.</returns>
    /// <exception cref="ScanException">Thrown when the scan operation fails.</exception>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IDocument> ScanAsync(CancellationToken cancellationToken)
    {
        _scanCompleteTaskSource = new();

        DispatchQueue.MainQueue.DispatchAsync(async () =>
        {
            try
            {
                using VNDocumentCameraViewController scanner = [];
                scanner.Delegate = this;
                scanner.ModalInPresentation = true;

                UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new ScanException("Failed to find top UIViewController.");

                await topViewController.PresentViewControllerAsync(scanner, true).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                _scanCompleteTaskSource.TrySetException(e);
            }
        });

        try
        {
            return await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception e)
        {
            throw new ScanException(e.Message, e);
        }
    }

    /// <summary>
    /// Called when the document scanning completes successfully.
    /// </summary>
    /// <param name="controller">The document camera view controller.</param>
    /// <param name="scan">The scanned document containing the captured pages.</param>
    public override void DidFinish(VNDocumentCameraViewController controller, VNDocumentCameraScan scan)
    {
        List<IDocumentPage> pages = [];

        for (nuint i = 0; i < scan.PageCount; i++)
        {
            pages.Add(new DocumentPage(scan.GetImage(i).AsPNG()?.ToArray() ?? []));
        }

        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(new Document(pages)));
    }

    /// <summary>
    /// Called when the user cancels the document scanning operation.
    /// </summary>
    /// <param name="controller">The document camera view controller.</param>
    public override void DidCancel(VNDocumentCameraViewController controller)
    {
        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(new Document(Enumerable.Empty<IDocumentPage>())));
    }

    /// <summary>
    /// Called when the document scanning operation fails with an error.
    /// </summary>
    /// <param name="controller">The document camera view controller.</param>
    /// <param name="error">The error that caused the failure.</param>
    public override void DidFail(VNDocumentCameraViewController controller, NSError error)
    {
        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetException(new ScanException(error.Description)));
    }
}
