namespace Plugin.Scanner.Android.Extensions;

internal static class RectExtensions
{
    public static bool ContainsWithTolerance(this Rect bounds, int x, int y, int tolerance)
    {
        using Rect expanded = new(bounds);
        expanded.Inset(-tolerance, -tolerance);

        return expanded.Contains(x, y);
    }
}
