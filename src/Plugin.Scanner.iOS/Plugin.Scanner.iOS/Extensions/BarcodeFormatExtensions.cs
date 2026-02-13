using Plugin.Scanner.Core.Barcode;
using Vision;

namespace Plugin.Scanner.iOS.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="BarcodeFormat"/> values to iOS Vision framework barcode symbology.
/// </summary>
internal static class BarcodeFormatExtensions
{
    /// <summary>
    /// Converts the specified <see cref="BarcodeFormat"/> flags to a collection of Vision framework barcode symbology strings.
    /// </summary>
    /// <param name="formats">The <see cref="BarcodeFormat"/> flags to convert. Can be a single format or multiple formats combined with bitwise OR.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/> containing the Vision framework symbology names corresponding to the specified formats.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported <see cref="BarcodeFormat"/> flag is encountered.</exception>
    /// <remarks>
    /// <para>
    /// This method maps cross-platform <see cref="BarcodeFormat"/> values to iOS-specific <see cref="VNBarcodeSymbology"/> values.
    /// </para>
    /// <para>
    /// If <see cref="BarcodeFormat.All"/> is specified, all available barcode formats are returned.
    /// Otherwise, only the formats specified by the flags are converted.
    /// </para>
    /// <para>
    /// Supported barcode formats include: Aztec, Codabar, Code39, Code93, Code128, DataMatrix, EAN-8, EAN-13, ITF-14, PDF417, QR, and UPC-E.
    /// </para>
    /// </remarks>
    public static IEnumerable<string> ToBarcodeFormats(this BarcodeFormat formats)
    {
        Func<BarcodeFormat, VNBarcodeSymbology> select = flag => flag switch
        {
            BarcodeFormat.Aztec => VNBarcodeSymbology.Aztec,
            BarcodeFormat.Codabar => VNBarcodeSymbology.Codabar,
            BarcodeFormat.Code39 => VNBarcodeSymbology.Code39,
            BarcodeFormat.Code93 => VNBarcodeSymbology.Code93,
            BarcodeFormat.Code128 => VNBarcodeSymbology.Code128,
            BarcodeFormat.DataMatrix => VNBarcodeSymbology.DataMatrix,
            BarcodeFormat.Ean8 => VNBarcodeSymbology.Ean8,
            BarcodeFormat.Ean13 => VNBarcodeSymbology.Ean13,
            BarcodeFormat.Itf14 => VNBarcodeSymbology.Itf14,
            BarcodeFormat.Pdf417 => VNBarcodeSymbology.Pdf417,
            BarcodeFormat.QR => VNBarcodeSymbology.QR,
            BarcodeFormat.Upce => VNBarcodeSymbology.Upce,
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null),
        };

        IEnumerable<VNBarcodeSymbology> symbologies;

        if (formats == BarcodeFormat.All)
        {
            symbologies = Enum.GetValues<BarcodeFormat>()
                .Where(flag => flag != BarcodeFormat.All)
                .Select(select);
        }
        else
        {
            symbologies = Enum.GetValues<BarcodeFormat>()
                .Where(flag => flag != BarcodeFormat.All && formats.HasFlag(flag))
                .Select(select);
        }

        return symbologies.Select(x => x.ToString());
    }
}