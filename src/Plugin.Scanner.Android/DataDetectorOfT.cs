using AndroidX.Camera.MLKit.Vision;
using AndroidX.Core.Util;
using Java.Interop;
using Java.Util;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android;

internal abstract class DataDetector<TResult> : DataDetector, IConsumer
    where TResult : class
{
    private const int MaxFrames = 5;
    private const int MinOccurrences = 3;

    private readonly List<RecognizedItem> _recognizedItems = new();
    private readonly Dictionary<RecognizedItem, int> _itemFrequencies = new();
    private int _frameCount;

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
            _frameCount = 0;
            Cleared?.Invoke(this, EventArgs.Empty);
            _recognizedItems.Clear();
        }
        else
        {
            _frameCount++;

            IEnumerable<TResult> detectedItems = results.ToEnumerable().OfType<TResult>();
            IReadOnlyList<RecognizedItem> recognizedItems = GetRecognizedItems(detectedItems);

            foreach (RecognizedItem item in recognizedItems)
            {
                if (_itemFrequencies.TryGetValue(item, out int value))
                {
                    _itemFrequencies[item] = ++value;
                }
                else
                {
                    _itemFrequencies[item] = 1;
                }
            }

            if (_frameCount >= MaxFrames)
            {
                List<RecognizedItem> frequentItems = _itemFrequencies
                    .Where(kv => kv.Value > MinOccurrences)
                    .Select(kv => kv.Key)
                    .ToList();

                if (frequentItems.Count != 0)
                {
                    Detected?.Invoke(this, frequentItems);
                }

                _frameCount = 0;
                _itemFrequencies.Clear();
            }
        }
    }
}