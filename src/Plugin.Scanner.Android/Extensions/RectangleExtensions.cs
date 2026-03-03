using System.Drawing;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="Rectangle"/> to Android <see cref="Rect"/>.
/// </summary>
internal static class RectangleExtensions
{
    /// <summary>
    /// Converts a <see cref="Rectangle"/> to an Android <see cref="Rect"/> with pixel coordinates.
    /// </summary>
    /// <param name="rect">The rectangle to convert.</param>
    /// <param name="context">The Android context used for density conversion.</param>
    /// <returns>An Android <see cref="Rect"/> with pixel-based coordinates.</returns>
    public static Rect ToRectPixel(this Rectangle rect, Context context)
    {
        return new Rect(
            Convert.ToInt32(context.ToPixels(rect.X)),
            Convert.ToInt32(context.ToPixels(rect.Y)),
            Convert.ToInt32(context.ToPixels(rect.Right)),
            Convert.ToInt32(context.ToPixels(rect.Bottom)));
    }

    /// <summary>
    /// Converts a <see cref="Rectangle"/> to an Android <see cref="Rect"/> without density conversion.
    /// </summary>
    /// <param name="rect">The rectangle to convert.</param>
    /// <returns>An Android <see cref="Rect"/> with the same coordinates.</returns>
    public static Rect ToRect(this Rectangle rect)
    {
        return new Rect(
            rect.X,
            rect.Y,
            rect.Right,
            rect.Bottom);
    }
}
