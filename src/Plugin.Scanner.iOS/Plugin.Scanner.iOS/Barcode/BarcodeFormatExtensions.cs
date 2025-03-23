using Plugin.Scanner.Core.Barcode;
using Vision;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <summary>
/// <see cref="BarcodeFormat"/> extension methods.
/// </summary>
internal static class BarcodeFormatExtensions
{
    /// <summary>
    /// Gets <see cref="VNBarcodeSymbology"/> for <see cref="BarcodeFormat"/>.
    /// </summary>
    /// <param name="formats">Formats.</param>
    /// <returns>Corresponding <see cref="VNBarcodeSymbology"/>.</returns>
    internal static IEnumerable<VNBarcodeSymbology> ToBarcodeFormats(this IEnumerable<string> formats)
    {
        return formats.Select(format => format.ToBarcodeFormat()).ToList();
    }

    /// <summary>
    /// Gets <see cref="VNBarcodeSymbology"/> for <see cref="BarcodeFormat"/>.
    /// </summary>
    /// <param name="format">Format.</param>
    /// <returns>Android format.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if no corresponding <see cref="VNBarcodeSymbology"/> for <see cref="BarcodeFormat"/> is found.</exception>
    internal static VNBarcodeSymbology ToBarcodeFormat(this string format)
    {
        return format switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };
    }
}