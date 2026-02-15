using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.Barcode;

internal sealed class BarcodeDetector : DataDetector<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>
{
    public BarcodeDetector(IDetector detector)
        : base(detector)
    {
    }

    protected override IReadOnlyList<RecognizedItem> GetRecognizedItems(IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode> detectedItems)
    {
        return detectedItems
            .Where(x => x.DisplayValue is not null && x.BoundingBox is not null)
            .Select(x => new RecognizedItem(x.DisplayValue!, x.BoundingBox!))
            .ToList();
    }
}