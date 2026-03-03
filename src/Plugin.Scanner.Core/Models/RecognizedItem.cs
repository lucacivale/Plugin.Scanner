using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace Plugin.Scanner.Core.Models;

/// <summary>
/// Represents an item recognized during scanning with text content and location bounds.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RecognizedItem : IEquatable<RecognizedItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItem"/> class with auto-generated ID.
    /// </summary>
    /// <param name="text">The recognized text content.</param>
    /// <param name="bounds">The bounding rectangle of the recognized item.</param>
    public RecognizedItem(string text, Rectangle bounds)
    {
        Text = text;
        Bounds = bounds;
        Id = GenerateIdFromText(text);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItem"/> class with a specified ID.
    /// </summary>
    /// <param name="id">The unique identifier for the item.</param>
    /// <param name="text">The recognized text content.</param>
    /// <param name="bounds">The bounding rectangle of the recognized item.</param>
    public RecognizedItem(Guid id, string text, Rectangle bounds)
        : this(text, bounds)
    {
        Id = id.ToString();
    }

    /// <summary>
    /// Gets an empty recognized item with no text or bounds.
    /// </summary>
    public static RecognizedItem Empty => new(string.Empty, Rectangle.Empty);

    /// <summary>
    /// Gets the unique identifier for this recognized item.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the recognized text content.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the bounding rectangle of the recognized item.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// Determines whether the specified <see cref="RecognizedItem"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The item to compare with the current instance.</param>
    /// <returns><c>true</c> if the items have the same ID; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj)
            || (obj is RecognizedItem other
                && Equals(other));
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code based on the item's ID.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Generates a unique SHA-256 hash-based identifier from the given text.
    /// </summary>
    /// <param name="text">The text to generate an ID from.</param>
    /// <returns>A hexadecimal string representing the SHA-256 hash of the text.</returns>
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