using Android.Gms.Tasks;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using Google.MLKit.Vision.Documentscanner;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using CancellationToken = System.Threading.CancellationToken;
using GmsTask = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android.Scanners;

internal sealed class DocumentScanner : Java.Lang.Object, IOnSuccessListener, IOnFailureListener, IActivityResultCallback
{
    private readonly ICurrentActivity _currentActivity;
    private readonly ActivityResultLauncher _scannerLauncher;
    private readonly ActivityResultContracts.StartIntentSenderForResult _startIntentSenderForResult;

    private TaskCompletionSource<IReadOnlyList<IDocument>> _taskCompletionSource;

    public DocumentScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;

        if (_currentActivity.Activity is AppCompatActivity activity)
        {
            _startIntentSenderForResult = new();
            _scannerLauncher = activity.RegisterForActivityResult(_startIntentSenderForResult, this);
        }
    }

    public async Task<IReadOnlyList<IDocument>> ScanAsync(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<IReadOnlyList<IDocument>>();

        using GmsDocumentScannerOptions.Builder builder = new();
        using GmsDocumentScannerOptions options = builder
          .SetGalleryImportAllowed(false)
          .SetResultFormats(GmsDocumentScannerOptions.ResultFormatPdf)
          .SetScannerMode(GmsDocumentScannerOptions.ScannerModeFull)
          .Build();

        IGmsDocumentScanner scanner = GmsDocumentScanning.GetClient(options);
        GmsTask intentSender = scanner.GetStartScanIntent(_currentActivity.Activity);
        intentSender.AddOnSuccessListener(this);
        intentSender.AddOnFailureListener(this);

        return await _taskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
    }

    public void OnSuccess(Java.Lang.Object? result)
    {
        if (result is IntentSender intent)
        {
            using IntentSenderRequest.Builder builder = new(intent);
            _scannerLauncher.Launch(builder.Build());
        }
    }

    public void OnFailure(Java.Lang.Exception e)
    {
        _taskCompletionSource.TrySetException(new ScanException("Something went wrong during scanning.", e));
    }

    public async void OnActivityResult(Java.Lang.Object? result)
    {
        if (result is not ActivityResult aResult
            || aResult.ResultCode != (int)Result.Ok
            || GmsDocumentScanningResult.FromActivityResultIntent(aResult.Data) is not GmsDocumentScanningResult scanResult
            || scanResult.Pages is not IList<GmsDocumentScanningResult.Page> pages)
        {
            _taskCompletionSource.TrySetException(new ScanException("Something went wrong during scanning."));
            return;
        }

        List<IDocument> documents = [];

        foreach (GmsDocumentScanningResult.Page page in pages)
        {
            using Stream? stream = _currentActivity.Activity.ContentResolver?.OpenInputStream(page.ImageUri);

            if (stream is not null)
            {
                using MemoryStream memoryStream = new();
                await stream.CopyToAsync(memoryStream).ConfigureAwait(true);

                documents.Add(new Document(memoryStream.ToArray()));
            }
        }

        _taskCompletionSource.TrySetResult(documents);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _startIntentSenderForResult.Dispose();
            _scannerLauncher.Dispose();
        }

        base.Dispose(disposing);
    }
}