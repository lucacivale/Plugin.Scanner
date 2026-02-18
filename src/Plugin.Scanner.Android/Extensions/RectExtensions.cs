namespace Plugin.Scanner.Android.Extensions;

internal static class RectExtensions
{
    public static bool ContainsWithTolerance(this Rect bounds, int x, int y, int tolerance)
    {
        using Rect expanded = new(bounds);
        expanded.Inset(-tolerance, -tolerance);

        return expanded.Contains(x, y);
    }

    public static bool ContainsWithTolerance(this Rect bounds, Rect other, float tolerance)
    {
        return Math.Abs(bounds.Left - other.Left) <= tolerance
            && Math.Abs(bounds.Top - other.Top) <= tolerance
            && Math.Abs(bounds.Width() - other.Width()) <= tolerance
            && Math.Abs(bounds.Height() - other.Height()) <= tolerance;
    }
}
