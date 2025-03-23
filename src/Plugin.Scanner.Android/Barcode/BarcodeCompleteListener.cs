using Android.Gms.Tasks;
using Plugin.Scanner.Core.Exceptions;
using Task = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android.Barcode;

/// <inheritdoc cref="Java.Lang.Object" />
internal sealed class BarcodeCompleteListener : Java.Lang.Object, IOnCompleteListener
{
    private readonly TaskCompletionSource<string> _taskCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeCompleteListener"/> class.
    /// </summary>
    /// <param name="taskCompletionSource"><see cref="TaskCompletionSource{TResult}"/>.</param>
    public BarcodeCompleteListener(TaskCompletionSource<string> taskCompletionSource)
    {
        _taskCompletionSource = taskCompletionSource;
    }

    /// <inheritdoc />
    public void OnComplete(Task task)
    {
        Exception? exception = null;

        if (task.IsSuccessful)
        {
            _taskCompletionSource.TrySetResult((task.Result as Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode)?.RawValue ?? string.Empty);
        }
        else if (task.IsCanceled)
        {
            exception = new TaskCanceledException("Operation cancelled.");
        }
        else if (task.Exception is not null)
        {
            exception = task.Exception;
        }

        if (exception is not null)
        {
            _taskCompletionSource.TrySetException(new BarcodeScanException("Barcode scan failed.", exception));
        }
    }
}