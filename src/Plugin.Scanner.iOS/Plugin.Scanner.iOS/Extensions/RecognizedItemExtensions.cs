using Plugin.Scanner.iOS.Binding;

namespace Plugin.Scanner.iOS.Extensions;

/// <summary>
/// Provides extension methods for <see cref="RecognizedItem"/> conversion.
/// </summary>
internal static class RecognizedItemExtensions
{
    /// <summary>
    /// Converts a platform-specific <see cref="RecognizedItem"/> to a core model <see cref="Core.Models.RecognizedItem"/>.
    /// </summary>
    /// <param name="item">The recognized item to convert.</param>
    /// <returns>A core model representation of the recognized item with normalized bounds.</returns>
    public static Core.Models.RecognizedItem ToRecognizedItem(this RecognizedItem item)
    {
        CGRect rect = new(item.Bounds.BottomLeft, new(item.Bounds.TopRight.X - item.Bounds.TopLeft.X, item.Bounds.TopRight.Y - item.Bounds.BottomRight.Y));

        return new(new Guid(item.Id.GetBytes()), item.Value, new((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
    }
}
