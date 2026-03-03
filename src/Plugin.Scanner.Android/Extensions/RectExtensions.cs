using System.Drawing;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for converting Android <see cref="Rect"/> to <see cref="Rectangle"/>.
/// </summary>
internal static class RectExtensions
{
    /// <summary>
    /// Converts an Android <see cref="Rect"/> to a <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="bounds">The Android rectangle to convert.</param>
    /// <returns>A <see cref="Rectangle"/> with equivalent dimensions.</returns>
    public static Rectangle ToRect(this Rect bounds)
    {
        return new Rectangle(bounds.Left, bounds.Top, bounds.Width(), bounds.Height());
    }
}
