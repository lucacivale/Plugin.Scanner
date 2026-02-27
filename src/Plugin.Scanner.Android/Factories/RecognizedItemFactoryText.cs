using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Text;

namespace Plugin.Scanner.Android.Factories;

internal sealed class RecognizedItemFactoryText : IRecognizedItemFactory<Text>
{
    public IReadOnlyList<RecognizedItem>? Create(Text? detectedItems)
    {
        if (detectedItems?.GetText() is null
            || detectedItems.TextBlocks.FirstOrDefault()?.BoundingBox is not Rect box)
        {
            return null;
        }

        return [new RecognizedItem(detectedItems.GetText(), box.ToRect())];
    }
}