namespace Plugin.Scanner.Android.Extensions;

internal static class CoreRectExtensions
{
    public static Rect ToRect(this Core.Models.Rect rect, Context context)
    {
        return new Rect(
            Convert.ToInt32(context.ToPixels(rect.X)),
            Convert.ToInt32(context.ToPixels(rect.Y)),
            Convert.ToInt32(context.ToPixels(rect.Right)),
            Convert.ToInt32(context.ToPixels(rect.Bottom)));
    }
}
