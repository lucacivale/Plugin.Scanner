using System.ComponentModel;
using Plugin.Scanner.Android.Extensions;

namespace Plugin.Scanner.Android;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RecognizedItem : IEquatable<RecognizedItem>
{
    public RecognizedItem(string text, Rect bounds)
    {
        Text = text;
        Bounds = bounds;
    }

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

        return Text == other.Text
            && Bounds.Equals(other.Bounds);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj)
            || (obj is RecognizedItem other
                && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text, Bounds);
    }

    public static RecognizedItem Empty => new(string.Empty, new Rect(0, 0, 0, 0));
}