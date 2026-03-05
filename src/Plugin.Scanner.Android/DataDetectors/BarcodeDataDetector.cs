using Android.Gms.Tasks;
using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Java.Util;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;
using MLBarcode = Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode;

namespace Plugin.Scanner.Android.DataDetectors;

/// <summary>
/// Detects barcodes in camera frames using ML Kit barcode scanning with frequency-based validation.
/// </summary>
internal sealed class BarcodeDataDetector : DataDetector<IEnumerable<MLBarcode>>, IOnSuccessListener
{
    private const int MaxFrames = 5;
    private const int MinOccurrences = 3;

    private readonly Dictionary<RecognizedItem, int> _itemFrequencies = new();
    private readonly IBarcodeScanner _barcodeScanner;

    private int _frameCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeDataDetector"/> class.
    /// </summary>
    /// <param name="detector">The ML Kit barcode detector instance.</param>
    /// <param name="recognizedItemFactory">The factory for creating recognized barcode items.</param>
    public BarcodeDataDetector(IBarcodeScanner detector, IRecognizedItemFactory<IEnumerable<MLBarcode>> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
        _barcodeScanner = detector;
    }

    /// <summary>
    /// Processes the ML Kit analyzer result and extracts recognized barcodes.
    /// </summary>
    /// <param name="t">The analyzer result object.</param>
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

    public void OnSuccess(Java.Lang.Object? result)
    {
        Accept(result);
    }

    public void Process(InputImage inputImage)
    {
        _ = _barcodeScanner.Process(inputImage).AddOnSuccessListener(this);
    }

    /// <summary>
    /// Processes recognized barcode items using frequency tracking to filter out false positives.
    /// Items must appear in multiple frames before being reported as detected.
    /// </summary>
    /// <param name="recognizedItems">The list of recognized barcode items.</param>
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