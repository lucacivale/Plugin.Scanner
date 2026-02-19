using System.Drawing;

namespace Plugin.Scanner.iOS.Extensions;

internal static class RectangleExtensions
{
    public static CGRect ToRect(this Rectangle rect)
    {
        return new CGRect(
            rect.X,
            rect.Y,
            rect.Width,
            rect.Height);
    }
}
