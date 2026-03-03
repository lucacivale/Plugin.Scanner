using System.Drawing;

namespace Plugin.Scanner.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Rectangle"/>.
/// </summary>
internal static class RectangleExtensions
{
    /// <summary>
    /// Determines whether the rectangle contains the specified point with a tolerance margin.
    /// </summary>
    /// <param name="bounds">The rectangle to check.</param>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <param name="tolerance">The tolerance margin to expand the rectangle by in all directions.</param>
    /// <returns><c>true</c> if the expanded rectangle contains the point; otherwise, <c>false</c>.</returns>
    public static bool ContainsWithTolerance(this Rectangle bounds, int x, int y, int tolerance)
    {
        Rectangle expanded = new(bounds.Location, bounds.Size);
        expanded.Inflate(tolerance, tolerance);

        return expanded.Contains(x, y);
    }
}
