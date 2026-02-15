using AndroidX.Camera.MLKit.Vision;
using AndroidX.Core.Util;
using Java.Interop;
using Java.Util;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android;

internal abstract class DataDetector<TResult> : DataDetector, IConsumer
    where TResult : class
{
    private readonly List<RecognizedItem> _recognizedItems = new();

    protected DataDetector(IDetector detector)
        : base(detector)
    {
    }

    public void Accept(Java.Lang.Object? t)
    {
        if (t is not MlKitAnalyzer.Result result)
        {
            return;
        }

        using (result)
        {
            ArrayList? results = null;
            result.GetValue(Detector)?.TryJavaCast(out results);

            using (results)
            {
                ProcessResults(results);
            }
        }
    }

    protected abstract IReadOnlyList<RecognizedItem> GetRecognizedItems(IEnumerable<TResult> detectedItems);

    private void ProcessResults(ArrayList? results)
    {
        if (results is null
            || results.IsEmpty)
        {
            Removed?.Invoke(this, (_recognizedItems, []));
            _recognizedItems.Clear();
        }
        else
        {
            IEnumerable<TResult> detectedItems = results.ToEnumerable().OfType<TResult>();
            IReadOnlyList<RecognizedItem> recognizedItems = GetRecognizedItems(detectedItems);

            List<RecognizedItem> removedItems = _recognizedItems.Where(x => recognizedItems.Any(y => y.Equals(x) == false)).ToList();
            List<RecognizedItem> addedItems = _recognizedItems.Count == 0 ? recognizedItems.ToList() : recognizedItems.Where(x => _recognizedItems.Any(y => y.Equals(x) == false)).ToList();

            _recognizedItems.Clear();
            _recognizedItems.AddRange(recognizedItems);

            if (removedItems.Count != 0)
            {
                Removed?.Invoke(this, (removedItems, _recognizedItems));
            }

            if (addedItems.Count != 0)
            {
               Added?.Invoke(this, (addedItems, _recognizedItems));
            }
        }
    }
}