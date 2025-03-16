namespace Plugin.Scanner.iOS;

/// <summary>
/// Types of text that a data scanner recognizes.
/// </summary>
public enum TextContentType
{
    Default,
    DateTimeDuration,
    EmailAddress,
    FlightNumber,
    FullStreetAddress,
    ShipmentTrackingNumber,
    TelephoneNumber,
    Url,
    Currency,
}

/// <summary>
/// <see cref="TextContentType"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1649: File name should match first type name", Justification = "Is ok here.")]
public static class TextContentTypeExtensions
{
    /// <summary>
    /// Converts text content type to native text content type.
    /// </summary>
    /// <param name="textContentType">Type to convert.</param>
    /// <returns>Native text content type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if no matching text content type is found.</exception>
    public static Plugin.Scanner.iOS.Binding.TextContentType ToVnTextContentType(this TextContentType textContentType)
    {
        return textContentType switch
        {
            TextContentType.Default => Plugin.Scanner.iOS.Binding.TextContentType.Default,
            TextContentType.DateTimeDuration => Plugin.Scanner.iOS.Binding.TextContentType.DateTimeDuration,
            TextContentType.EmailAddress => Plugin.Scanner.iOS.Binding.TextContentType.EmailAddress,
            TextContentType.FlightNumber => Plugin.Scanner.iOS.Binding.TextContentType.FlightNumber,
            TextContentType.FullStreetAddress => Plugin.Scanner.iOS.Binding.TextContentType.FullStreetAddress,
            TextContentType.ShipmentTrackingNumber => Plugin.Scanner.iOS.Binding.TextContentType.ShipmentTrackingNumber,
            TextContentType.TelephoneNumber => Plugin.Scanner.iOS.Binding.TextContentType.TelephoneNumber,
            TextContentType.Url => Plugin.Scanner.iOS.Binding.TextContentType.Url,
            TextContentType.Currency => Plugin.Scanner.iOS.Binding.TextContentType.Currency,
            _ => throw new ArgumentOutOfRangeException(nameof(textContentType), textContentType, null),
        };
    }
}