#pragma warning disable IDE0005
using System;
using System.Collections.Generic;
#pragma warning restore IDE0005

namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Supported barcode formats.
/// </summary>
public static class BarcodeFormat
{
    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Aztec-Code"/>.
    /// </summary>
    public const string Aztec = "Aztec";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Codabar"/>.
    /// </summary>
    public const string Codabar = "Codabar";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Code_39"/>.
    /// </summary>
    public const string Code39 = "Code39";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Code_93"/>.
    /// </summary>
    public const string Code93 = "Code93";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Code128"/>.
    /// </summary>
    public const string Code128 = "Code128";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Data_Matrix"/>.
    /// </summary>
    public const string DataMatrix = "DataMatrix";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/European_Article_Number"/>.
    /// </summary>
    public const string Ean8 = "Ean8";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/European_Article_Number"/>.
    /// </summary>
    public const string Ean13 = "Ean13";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Aztec-Code"/>.
    /// </summary>
    public const string Itf14 = "Itf14";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/PDF417"/>.
    /// </summary>
    public const string Pdf417 = "Pdf417";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/QR-Code"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public const string QR = "QR";

    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/Universal_Product_Code"/>.
    /// </summary>
    public const string Upce = "Upce";

    /// <summary>
    /// All supported barcode formats.
    /// </summary>
    /// <returns>Collection of supported barcode formats.</returns>
    public static IEnumerable<string> SupportedBarcodeFormats()
    {
        return [
            Aztec,
            Codabar,
            Code39,
            Code93,
            Code128,
            DataMatrix,
            Ean8,
            Ean13,
            Itf14,
            Pdf417,
            QR,
            Upce,
        ];
    }
}