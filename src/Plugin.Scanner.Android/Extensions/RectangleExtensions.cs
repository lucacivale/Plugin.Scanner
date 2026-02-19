using System.Drawing;

namespace Plugin.Scanner.Android.Extensions;

internal static class RectangleExtensions
{
    public static Rect ToRect(this Rectangle rect, Context context)
    {
        return new Rect(
            Convert.ToInt32(context.ToPixels(rect.X)),
            Convert.ToInt32(context.ToPixels(rect.Y)),
            Convert.ToInt32(context.ToPixels(rect.Right)),
            Convert.ToInt32(context.ToPixels(rect.Bottom)));
    }
}
