#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// Types of text that a data scanner recognizes.
/// </summary>
public enum TextContentType
{
    /// <summary>
    /// Default
    /// </summary>
    Default,

    /// <summary>
    /// The content type for dates, times, and durations that appear in text.
    /// </summary>
    DateTimeDuration,

    /// <summary>
    /// The content type for an email address that appears in text.
    /// </summary>
    EmailAddress,

    /// <summary>
    /// The content type for a vendor-specific flight number that appears in text.
    /// </summary>
    FlightNumber,

    /// <summary>
    /// The content type for a mailing address that appears in text.
    /// </summary>
    FullStreetAddress,

    /// <summary>
    /// The content type for a vendor-specific parcel tracking number that appears in text.
    /// </summary>
    ShipmentTrackingNumber,

    /// <summary>
    /// The content type for a phone number that appears in text.
    /// </summary>
    TelephoneNumber,

    /// <summary>
    /// The content type for a URL that appears in text.
    /// </summary>
    Url,

    /// <summary>
    /// The content type for currency.
    /// </summary>
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