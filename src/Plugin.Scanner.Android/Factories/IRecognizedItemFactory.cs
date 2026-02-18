using Plugin.Scanner.Android.Models;

namespace Plugin.Scanner.Android.Factories;

internal interface IRecognizedItemFactory<TDetectedItemsType>
    where TDetectedItemsType : class
{
    public IReadOnlyList<RecognizedItem> Create(IEnumerable<TDetectedItemsType?> detectedItems);
}