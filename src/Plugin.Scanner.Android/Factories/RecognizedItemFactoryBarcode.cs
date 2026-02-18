using Plugin.Scanner.Android.Models;

namespace Plugin.Scanner.Android.Factories;

internal class RecognizedItemFactoryBarcode : IRecognizedItemFactory<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>
{
    public IReadOnlyList<RecognizedItem> Create(IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode?> detectedItems)
    {
        return detectedItems
            .Where(x => x?.DisplayValue is not null && x?.BoundingBox is not null)
            .Select(x => new RecognizedItem(x!.DisplayValue!, x!.BoundingBox!))
            .ToList();
    }
}
