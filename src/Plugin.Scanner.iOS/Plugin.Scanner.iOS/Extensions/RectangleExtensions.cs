using System.Drawing;

namespace Plugin.Scanner.iOS.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Rectangle"/> conversion.
/// </summary>
internal static class RectangleExtensions
{
    /// <summary>
    /// Converts a <see cref="Rectangle"/> to a <see cref="CGRect"/>.
    /// </summary>
    /// <param name="rect">The rectangle to convert.</param>
    /// <returns>A <see cref="CGRect"/> with the same dimensions and position.</returns>
    public static CGRect ToRect(this Rectangle rect)
    {
        return new CGRect(
            rect.X,
            rect.Y,
            rect.Width,
            rect.Height);
    }
}
