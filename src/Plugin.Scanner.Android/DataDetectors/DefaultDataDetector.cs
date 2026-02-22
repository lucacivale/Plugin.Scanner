using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Java.Util;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.DataDetectors;

internal sealed class DefaultDataDetector<TDetectedItemsType> : Java.Lang.Object, IDataDetector
    where TDetectedItemsType : class
{
    private const int MaxFrames = 5;
    private const int MinOccurrences = 3;

    private readonly IDetector _detector;
    private readonly IRecognizedItemFactory<TDetectedItemsType> _recognizedItemFactory;
    private readonly Dictionary<RecognizedItem, int> _itemFrequencies = new();

    private int _frameCount;

    public DefaultDataDetector(IDetector detector, IRecognizedItemFactory<TDetectedItemsType> recognizedItemFactory)
    {
        _detector = detector;
        _recognizedItemFactory = recognizedItemFactory;
    }

    public IDetector Detector => _detector;

    public Rect? RegionOfInterest { get; set; }

    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    public EventHandler? Cleared { get; set; }

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

    public void Stop()
    {
        _detector.Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _detector.Close();
            _detector.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ProcessResults(ArrayList? results)
    {
        if (results is null
             || results.IsEmpty)
        {
            _frameCount = 0;
            Cleared?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _frameCount++;

            IReadOnlyList<RecognizedItem> recognizedItems = _recognizedItemFactory.Create(results.ToEnumerable().OfType<TDetectedItemsType>());

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
                List<RecognizedItem> frequentItems;

                if (RegionOfInterest is not null)
                {
                    frequentItems = _itemFrequencies
                        .Where(kv => kv.Value > MinOccurrences && RegionOfInterest.Contains(kv.Key.Bounds.ToRect()))
                        .Select(kv => kv.Key)
                        .ToList();

                    if (frequentItems.Count == 0)
                    {
                        Cleared?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    frequentItems = _itemFrequencies
                        .Where(kv => kv.Value > MinOccurrences)
                        .Select(kv => kv.Key)
                        .ToList();
                }

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