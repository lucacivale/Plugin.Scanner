namespace Plugin.Scanner.iOS.Extensions;

internal static class CoreRectExtensions
{
    public static CGRect ToRect(this Core.Models.Rect rect)
    {
        return new CGRect(
            rect.X,
            rect.Y,
            rect.Width,
            rect.Height);
    }
}
