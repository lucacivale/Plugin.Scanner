namespace Plugin.Scanner.iOS.Extensions;

internal static class StringExtensions
{
    public static string Truncate(this string value, int limit)
    {
        return value.Length <= limit ? value : string.Concat(value.AsSpan(0, limit - 1), "…");
    }
}
