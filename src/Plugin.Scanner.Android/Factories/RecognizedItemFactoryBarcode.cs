using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Factories;

internal sealed class RecognizedItemFactoryBarcode : IRecognizedItemFactory<IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>>
{
    public IReadOnlyList<RecognizedItem>? Create(IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>? detectedItems)
    {
        return detectedItems?
            .Where(x => x.DisplayValue is not null && x.BoundingBox is not null)
            .Select(x => new RecognizedItem(x.DisplayValue!, x.BoundingBox!.ToRect()))
            .ToList();
    }
}
