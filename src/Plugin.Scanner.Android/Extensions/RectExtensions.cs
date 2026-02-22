using System.Drawing;

namespace Plugin.Scanner.Android.Extensions;

internal static class RectExtensions
{
    public static Rectangle ToRect(this Rect bounds)
    {
        return new Rectangle(bounds.Left, bounds.Top, bounds.Width(), bounds.Height());
    }
}
