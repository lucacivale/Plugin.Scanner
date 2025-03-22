#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// An object that represents the four corners of a recognized item.
/// </summary>
public sealed class Bounds
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bounds"/> class.
    /// </summary>
    /// <param name="topLeft">The upper-left corner of the recognized item in view coordinates.</param>
    /// <param name="topRight">The upper-right corner of the recognized item in view coordinates.</param>
    /// <param name="bottomLeft">The lower-left corner of the recognized item in view coordinates.</param>
    /// <param name="bottomRight">The lower-right corner of the recognized item in view coordinates.</param>
    public Bounds(CGPoint topLeft, CGPoint topRight, CGPoint bottomLeft, CGPoint bottomRight)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
    }

    /// <summary>
    /// Gets the upper-left corner of the recognized item in view coordinates.
    /// </summary>
    public CGPoint TopLeft { get; init; }

    /// <summary>
    /// Gets the upper-right corner of the recognized item in view coordinates.
    /// </summary>
    public CGPoint TopRight { get; init; }

    /// <summary>
    /// Gets the lower-left corner of the recognized item in view coordinates.
    /// </summary>
    public CGPoint BottomLeft { get; init; }

    /// <summary>
    /// Gets the lower-right corner of the recognized item in view coordinates.
    /// </summary>
    public CGPoint BottomRight { get; init; }
}

/// <summary>
/// <see cref="Bounds"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1649: File name should match first type name", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1402: A C# code file contains more than one unique type.", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1204: A static element is positioned beneath an instance element of the same type.", Justification = "Is ok here.")]
public static class BoundsExtensions
{
    /// <summary>
    /// To <see cref="Plugin.Scanner.iOS.Binding.Bounds"/>.
    /// </summary>
    /// <param name="bounds"><see cref="Bounds"/>.</param>
    /// <returns>Returns <see cref="Plugin.Scanner.iOS.Binding.Bounds"/>.</returns>
    public static Plugin.Scanner.iOS.Binding.Bounds ToVnBounds(this Bounds bounds)
    {
        return new(bounds.TopLeft, bounds.TopRight, bounds.BottomRight, bounds.BottomLeft);
    }

    /// <summary>
    /// To <see cref="Bounds"/>.
    /// </summary>
    /// <param name="bounds"><see cref="Plugin.Scanner.iOS.Binding.Bounds"/>.</param>
    /// <returns>Returns <see cref="Bounds"/>.</returns>
    public static Bounds ToBounds(this Plugin.Scanner.iOS.Binding.Bounds bounds)
    {
        return new(bounds.TopLeft, bounds.TopRight, bounds.BottomRight, bounds.BottomLeft);
    }
}