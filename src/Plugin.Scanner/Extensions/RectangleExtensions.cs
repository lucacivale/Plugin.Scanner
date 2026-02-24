using System.Drawing;

namespace Plugin.Scanner.Extensions;

internal static class RectangleExtensions
{
    public static bool ContainsWithTolerance(this Rectangle bounds, int x, int y, int tolerance)
    {
        Rectangle expanded = new(bounds.Location, bounds.Size);
        expanded.Inflate(tolerance, tolerance);

        return expanded.Contains(x, y);
    }
}
