namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Represents a barcode and provides access to its raw decoded value.
/// </summary>
/// <remarks>
/// This interface is implemented by platform-specific barcode objects and provides a consistent
/// way to access barcode data across different platforms.
/// </remarks>
public interface IBarcode
{
    /// <summary>
    /// Gets the raw string value decoded from the barcode.
    /// </summary>
    /// <value>
    /// The decoded string content of the scanned barcode. This represents the actual data
    /// encoded in the barcode without any additional processing or formatting.
    /// </value>
    /// <remarks>
    /// The format and content of this value depends on the barcode type. For example:
    /// <list type="bullet">
    /// <item><description>EAN/UPC codes contain numeric product identifiers</description></item>
    /// <item><description>QR codes can contain URLs, text, or structured data</description></item>
    /// <item><description>Code 39/128 can contain alphanumeric data</description></item>
    /// </list>
    /// </remarks>
    string RawValue { get; }
}