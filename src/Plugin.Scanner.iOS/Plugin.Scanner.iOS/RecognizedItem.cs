#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// An item that the data scanner recognizes in the camera’s live video.
/// </summary>
public sealed class RecognizedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItem"/> class.
    /// </summary>
    /// <param name="id">A unique identifier for the recognized item.</param>
    /// <param name="bounds">The bounds of the recognized item in view coordinates.</param>
    /// <param name="value">The string that the item represents.</param>
    public RecognizedItem(NSUuid id, Bounds bounds, string value)
    {
        Id = id;
        Bounds = bounds;
        Value = value;
    }

    /// <summary>
    /// Gets a unique identifier for the recognized item.
    /// </summary>
    public NSUuid Id { get; init; }

    /// <summary>
    /// Gets the bounds of the recognized item in view coordinates.
    /// </summary>
    public Bounds Bounds { get; init; }

    /// <summary>
    /// Gets the string that the item represents.
    /// </summary>
    public string Value { get; init; }
}

/// <summary>
/// <see cref="RecognizedItem"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1649: File name should match first type name", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1402: A C# code file contains more than one unique type.", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1204: A static element is positioned beneath an instance element of the same type.", Justification = "Is ok here.")]
public static class RecognizedItemExtensions
{
    /// <summary>
    /// To <see cref="RecognizedItem"/>.
    /// </summary>
    /// <param name="item"><see cref="Plugin.Scanner.iOS.Binding.RecognizedItem"/>.</param>
    /// <returns>Returns <see cref="RecognizedItem"/>.</returns>
    public static RecognizedItem ToRecognizedItem(this Plugin.Scanner.iOS.Binding.RecognizedItem item)
    {
        return new(item.Id, item.Bounds.ToBounds(), item.Value);
    }
}