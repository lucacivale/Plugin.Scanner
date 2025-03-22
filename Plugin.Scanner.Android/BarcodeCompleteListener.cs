using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Android.Gms.Tasks;
using Task = Android.Gms.Tasks.Task;

namespace Plugin.Scanner.Android;

/// <inheritdoc cref="Java.Lang.Object" />
public class BarcodeCompleteListener(TaskCompletionSource<string> taskCompletionSource) : Java.Lang.Object, IOnCompleteListener
{
    /// <inheritdoc />
    public void OnComplete(Task task)
    {
        if (task.IsSuccessful)
        {
            taskCompletionSource.TrySetResult((task.Result as Barcode)?.RawValue ?? string.Empty);
        }
        else if (task.IsCanceled)
        {
            taskCompletionSource.TrySetException(new TaskCanceledException("Operation cancelled."));
        }
        else if (task.Exception is not null)
        {
            taskCompletionSource.TrySetException(task.Exception);
        }
    }
}