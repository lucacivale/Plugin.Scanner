using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Factories;

/// <summary>
/// Factory for converting ML Kit barcode scanning results to recognized items.
/// </summary>
internal sealed class RecognizedItemFactoryBarcode : IRecognizedItemFactory<IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>>
{
    /// <summary>
    /// Creates a list of recognized items from ML Kit barcode detection results.
    /// </summary>
    /// <param name="detectedItems">The detected barcodes from ML Kit.</param>
    /// <returns>A read-only list of recognized barcode items with valid display values and bounding boxes.</returns>
    public IReadOnlyList<RecognizedItem>? Create(IEnumerable<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>? detectedItems)
    {
        return detectedItems?
            .Where(x => x.DisplayValue is not null && x.BoundingBox is not null)
            .Select(x => new RecognizedItem(x.DisplayValue!, x.BoundingBox!.ToRect()))
            .ToList();
    }
}
