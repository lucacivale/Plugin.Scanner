using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Factories;

internal interface IRecognizedItemFactory<TDetectedItemsType>
    where TDetectedItemsType : class
{
    IReadOnlyList<RecognizedItem>? Create(TDetectedItemsType? detectedItems);
}