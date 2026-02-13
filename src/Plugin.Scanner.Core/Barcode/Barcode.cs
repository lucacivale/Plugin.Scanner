namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Represents a scanned barcode containing its raw string value.
/// </summary>
public sealed class Barcode : IBarcode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Barcode"/> class with the specified raw value.
    /// </summary>
    /// <param name="rawValue">The raw string value decoded from the barcode.</param>
    public Barcode(string rawValue)
    {
        RawValue = rawValue;
    }

    /// <summary>
    /// Gets the raw string value of the barcode.
    /// </summary>
    /// <value>
    /// The decoded string content of the scanned barcode.
    /// </value>
    public string RawValue { get; }
}