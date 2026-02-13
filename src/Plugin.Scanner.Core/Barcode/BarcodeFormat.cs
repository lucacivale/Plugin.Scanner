namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Specifies the barcode formats that can be recognized by the scanner.
/// </summary>
/// <remarks>
/// This is a flags enumeration that allows combining multiple barcode formats using bitwise operations.
/// Use <see cref="All"/> to recognize all supported formats, or combine specific formats using the bitwise OR operator.
/// </remarks>
[Flags]
public enum BarcodeFormat
{
    /// <summary>
    /// Aztec 2D barcode format.
    /// </summary>
    Aztec = 1 << 0,

    /// <summary>
    /// Codabar 1D barcode format, commonly used in libraries, blood banks, and air parcel tracking.
    /// </summary>
    Codabar = 1 << 1,

    /// <summary>
    /// Code 39 1D barcode format, widely used in automotive and defense industries.
    /// </summary>
    Code39 = 1 << 2,

    /// <summary>
    /// Code 93 1D barcode format, designed to supplement and improve upon Code 39.
    /// </summary>
    Code93 = 1 << 3,

    /// <summary>
    /// Code 128 1D barcode format, high-density format used in logistics and transportation.
    /// </summary>
    Code128 = 1 << 4,

    /// <summary>
    /// Data Matrix 2D barcode format, commonly used for marking small items.
    /// </summary>
    DataMatrix = 1 << 5,

    /// <summary>
    /// EAN-8 barcode format, a shortened version of EAN-13 for small packages.
    /// </summary>
    Ean8 = 1 << 6,

    /// <summary>
    /// EAN-13 barcode format, the standard European retail product code.
    /// </summary>
    Ean13 = 1 << 7,

    /// <summary>
    /// ITF-14 barcode format, used for packaging and shipping containers.
    /// </summary>
    Itf14 = 1 << 8,

    /// <summary>
    /// PDF417 2D barcode format, capable of storing large amounts of data.
    /// </summary>
    Pdf417 = 1 << 9,

    /// <summary>
    /// QR Code 2D barcode format, widely used for URLs, payments, and general data encoding.
    /// </summary>
    QR = 1 << 10,

    /// <summary>
    /// UPC-E barcode format, a compressed version of UPC-A for small packages.
    /// </summary>
    Upce = 1 << 11,

    /// <summary>
    /// All supported barcode formats.
    /// </summary>
    /// <remarks>
    /// Use this value to enable recognition of all available barcode formats simultaneously.
    /// </remarks>
    All = Aztec | Codabar | Code39 | Code93 | Code128 | DataMatrix | Ean8 | Ean13 | Itf14 | Pdf417 | QR | Upce,
}