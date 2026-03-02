using CoreFoundation;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Scanners;
using System.Diagnostics.CodeAnalysis;
using VisionKit;

namespace Plugin.Scanner.iOS.Scanners;

internal sealed class DocumentScanner : VNDocumentCameraViewControllerDelegate, IDocumentScanner
{
    private TaskCompletionSource<IReadOnlyList<IDocument>>? _scanCompleteTaskSource;

    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IReadOnlyList<IDocument>> ScanAsync(CancellationToken cancellationToken)
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

    public override void DidFinish(VNDocumentCameraViewController controller, VNDocumentCameraScan scan)
    {
        List<IDocument> documents = [];

        for (nuint i = 0; i < scan.PageCount; i++)
        {
            documents.Add(new Document(scan.GetImage(i).AsPNG()?.ToArray() ?? []));
        }

        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(documents));
    }

    public override void DidCancel(VNDocumentCameraViewController controller)
    {
        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult([]));
    }

    public override void DidFail(VNDocumentCameraViewController controller, NSError error)
    {
        controller.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetException(new ScanException(error.Description)));
    }
}
