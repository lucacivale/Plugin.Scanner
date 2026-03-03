using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Android.Factories;

/// <summary>
/// Defines a contract for factories that convert ML Kit detection results to recognized items.
/// </summary>
/// <typeparam name="TDetectedItemsType">The type of items detected by ML Kit.</typeparam>
internal interface IRecognizedItemFactory<in TDetectedItemsType>
    where TDetectedItemsType : class
{
    /// <summary>
    /// Creates a list of recognized items from ML Kit detection results.
    /// </summary>
    /// <param name="detectedItems">The detected items from ML Kit.</param>
    /// <returns>A read-only list of recognized items, or <c>null</c> if no items were detected.</returns>
    IReadOnlyList<RecognizedItem>? Create(TDetectedItemsType? detectedItems);
}