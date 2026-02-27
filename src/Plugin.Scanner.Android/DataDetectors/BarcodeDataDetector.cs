using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Java.Util;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;
using MLBarcode = Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode;

namespace Plugin.Scanner.Android.DataDetectors;

internal sealed class BarcodeDataDetector: DataDetector<IEnumerable<MLBarcode>>
{
    public BarcodeDataDetector(IDetector detector, IRecognizedItemFactory<IEnumerable<MLBarcode>> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
    }

    protected override IReadOnlyList<RecognizedItem>? MlKitResultToRecognizedItems(MlKitAnalyzer.Result result)
    {
        ArrayList? results = null;
        result.GetValue(Detector)?.TryJavaCast(out results);

        using (results)
        {
            return RecognizedItemFactory.Create(results?.ToEnumerable().OfType<MLBarcode>() ?? Enumerable.Empty<MLBarcode>());
        }
    }
}