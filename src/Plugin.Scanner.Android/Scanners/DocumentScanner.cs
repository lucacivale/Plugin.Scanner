using System.Diagnostics.CodeAnalysis;
using Android.Gms.Tasks;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using Google.MLKit.Vision.Documentscanner;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Scanners;
using CancellationToken = System.Threading.CancellationToken;
using GmsTask = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android.Scanners;

/// <summary>
/// Provides Android-specific document scanning using Google ML Kit Document Scanner.
/// </summary>
internal sealed class DocumentScanner : Java.Lang.Object, IOnSuccessListener, IOnFailureListener, IActivityResultCallback, IDocumentScanner
{
    private readonly ICurrentActivity _currentActivity;
    private readonly ActivityResultLauncher? _scannerLauncher;
    private readonly ActivityResultContracts.StartIntentSenderForResult? _startIntentSenderForResult;

    private TaskCompletionSource<IDocument>? _taskCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentScanner"/> class.
    /// </summary>
    /// <param name="currentActivity">The current activity provider.</param>
    public DocumentScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;

        if (_currentActivity.Activity is AppCompatActivity activity)
        {
            _startIntentSenderForResult = new();
            _scannerLauncher = activity.RegisterForActivityResult(_startIntentSenderForResult, this);
        }
    }

    /// <summary>
    /// Scans a document using Google ML Kit Document Scanner.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task containing the scanned document with its pages.</returns>
    public async Task<IDocument> ScanAsync(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<IDocument>();

        using GmsDocumentScannerOptions.Builder builder = new();
        using GmsDocumentScannerOptions options = builder
          .SetGalleryImportAllowed(false)
          .SetResultFormats(GmsDocumentScannerOptions.ResultFormatJpeg)
          .SetScannerMode(GmsDocumentScannerOptions.ScannerModeFull)
          .Build();

        IGmsDocumentScanner scanner = GmsDocumentScanning.GetClient(options);
        GmsTask intentSender = scanner.GetStartScanIntent(_currentActivity.Activity);
        intentSender.AddOnSuccessListener(this);
        intentSender.AddOnFailureListener(this);

        return await _taskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
    }

    /// <summary>
    /// Handles successful retrieval of the scanner intent.
    /// </summary>
    /// <param name="result">The intent sender result.</param>
    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is IntentSender intent)
        {
            using IntentSenderRequest.Builder builder = new(intent);
            _scannerLauncher?.Launch(builder.Build());
        }
    }

    /// <summary>
    /// Handles failures during scanner initialization.
    /// </summary>
    /// <param name="e">The exception that occurred.</param>
    public void OnFailure(Java.Lang.Exception e)
    {
        _taskCompletionSource?.TrySetException(new ScanException("Something went wrong during scanning.", e));
    }

    /// <summary>
    /// Handles the activity result from the document scanner.
    /// </summary>
    /// <param name="result">The activity result containing scanned pages.</param>
    [SuppressMessage("Usage", "VSTHRD100:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    public async void OnActivityResult(Java.Lang.Object? result)
    {
        if (result is not ActivityResult aResult
            || aResult.ResultCode != (int)Result.Ok
            || GmsDocumentScanningResult.FromActivityResultIntent(aResult.Data) is not GmsDocumentScanningResult scanResult
            || scanResult.Pages is not IList<GmsDocumentScanningResult.Page> pages)
        {
            _taskCompletionSource?.TrySetResult(new Document(Enumerable.Empty<IDocumentPage>()));
            return;
        }

        List<IDocumentPage> documentPages = [];

        foreach (GmsDocumentScanningResult.Page page in pages)
        {
            using Stream? stream = _currentActivity.Activity.ContentResolver?.OpenInputStream(page.ImageUri);

            if (stream is not null)
            {
                using MemoryStream memoryStream = new();
                await stream.CopyToAsync(memoryStream).ConfigureAwait(true);

                documentPages.Add(new DocumentPage(memoryStream.ToArray()));
            }
        }

        _taskCompletionSource?.TrySetResult(new Document(documentPages));
    }

    /// <summary>
    /// Releases resources used by the document scanner.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _startIntentSenderForResult?.Dispose();
            _scannerLauncher?.Dispose();
        }

        base.Dispose(disposing);
    }
}