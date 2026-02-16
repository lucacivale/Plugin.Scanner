using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Plugin.Scanner.Android;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RecognizedItem : IEquatable<RecognizedItem>
{
    public RecognizedItem(string text, Rect bounds)
    {
        Text = text;
        Bounds = bounds;
        Id = GenerateIdFromText(text);
    }

    public string Id { get; }

    public string Text { get; }

    public Rect Bounds { get; }

    public bool Equals(RecognizedItem? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj)
            || (obj is RecognizedItem other
                && Equals(other));
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode(StringComparison.Ordinal);
    }

    public bool Compare(RecognizedItem? other)
    {
        return Text.Equals(other?.Text, StringComparison.Ordinal)
            && Bounds.Equals(other?.Bounds);
    }

    public static RecognizedItem Empty => new(string.Empty, new Rect(0, 0, 0, 0));

    private static string GenerateIdFromText(string text)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));

        StringBuilder sb = new();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2", System.Globalization.CultureInfo.CurrentCulture));
        }

        return sb.ToString();
    }
}