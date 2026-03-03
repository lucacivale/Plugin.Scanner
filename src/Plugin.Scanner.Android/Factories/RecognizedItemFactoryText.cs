using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Text;

namespace Plugin.Scanner.Android.Factories;

/// <summary>
/// Factory for converting ML Kit text recognition results to recognized items.
/// </summary>
internal sealed class RecognizedItemFactoryText : IRecognizedItemFactory<Text>
{
    /// <summary>
    /// Creates a list of recognized items from ML Kit text detection results.
    /// </summary>
    /// <param name="detectedItems">The detected text from ML Kit.</param>
    /// <returns>A read-only list of recognized text items with valid bounding boxes.</returns>
    public IReadOnlyList<RecognizedItem>? Create(Text? detectedItems)
    {
        return detectedItems?.TextBlocks
            .Where(x => x.BoundingBox is not null)
            .Select(x => new RecognizedItem(x.Text, x.BoundingBox!.ToRect()))
            .ToList();
    }
}