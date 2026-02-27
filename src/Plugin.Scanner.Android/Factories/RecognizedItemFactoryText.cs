using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Text;

namespace Plugin.Scanner.Android.Factories;

internal sealed class RecognizedItemFactoryText : IRecognizedItemFactory<Text>
{
    public IReadOnlyList<RecognizedItem>? Create(Text? detectedItems)
    {
        return detectedItems?.TextBlocks
            .Where(x => x.Text is not null && x.BoundingBox is not null)
            .Select(x => new RecognizedItem(x.Text!, x.BoundingBox!.ToRect()))
            .ToList();
    }
}