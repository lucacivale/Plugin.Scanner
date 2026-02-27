using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Java.Util;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;
using MLBarcode = Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode;

namespace Plugin.Scanner.Android.DataDetectors;

internal sealed class BarcodeDataDetector : DataDetector<IEnumerable<MLBarcode>>
{
    private const int MaxFrames = 5;
    private const int MinOccurrences = 3;

    private readonly Dictionary<RecognizedItem, int> _itemFrequencies = new();

    private int _frameCount;

    public BarcodeDataDetector(IDetector detector, IRecognizedItemFactory<IEnumerable<MLBarcode>> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
    }

    public override void Accept(Java.Lang.Object? t)
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
                ProcessResults(RecognizedItemFactory.Create(results?.ToEnumerable().OfType<MLBarcode>() ?? Enumerable.Empty<MLBarcode>()));
            }
        }
    }

    private void ProcessResults(IReadOnlyList<RecognizedItem>? recognizedItems)
    {
        if (recognizedItems is null
            || recognizedItems.Count == 0)
        {
            _frameCount = 0;
            Cleared?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _frameCount++;

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