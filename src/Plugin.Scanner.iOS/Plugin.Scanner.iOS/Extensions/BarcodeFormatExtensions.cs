using Plugin.Scanner.Core.Models.Enums;
using Vision;

namespace Plugin.Scanner.iOS.Extensions;

/// <summary>
/// Provides extension methods for <see cref="BarcodeFormat"/> conversion.
/// </summary>
internal static class BarcodeFormatExtensions
{
    /// <summary>
    /// Converts <see cref="BarcodeFormat"/> flags to Vision framework barcode symbology strings.
    /// </summary>
    /// <param name="formats">The barcode format flags to convert.</param>
    /// <returns>An enumerable collection of barcode symbology strings recognized by the Vision framework.</returns>
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