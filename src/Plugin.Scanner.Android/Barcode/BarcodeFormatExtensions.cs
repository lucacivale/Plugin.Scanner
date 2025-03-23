using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner.Android.Barcode;

/// <summary>
/// <see cref="BarcodeFormat"/> extension methods.
/// </summary>
internal static class BarcodeFormatExtensions
{
    /// <summary>
    /// Gets <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/> for <see cref="BarcodeFormat"/>.
    /// </summary>
    /// <param name="formats">Formats.</param>
    /// <returns>Corresponding <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/>.</returns>
    internal static IEnumerable<int> ToBarcodeFormats(this IEnumerable<string> formats)
    {
        return formats.Select(format => format.ToBarcodeFormat()).ToList();
    }

    /// <summary>
    /// Gets <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/> for <see cref="BarcodeFormat"/>.
    /// </summary>
    /// <param name="format">Format.</param>
    /// <returns>Android format.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if no corresponding <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/> for <see cref="BarcodeFormat"/> is found.</exception>
    internal static int ToBarcodeFormat(this string format)
    {
        return format switch
        {
            BarcodeFormat.Aztec => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatAztec,
            BarcodeFormat.Codabar => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatCodabar,
            BarcodeFormat.Code39 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatCode39,
            BarcodeFormat.Code93 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatCode93,
            BarcodeFormat.Code128 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatCode128,
            BarcodeFormat.DataMatrix => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatDataMatrix,
            BarcodeFormat.Ean8 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatEan8,
            BarcodeFormat.Ean13 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatEan13,
            BarcodeFormat.Itf14 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatItf,
            BarcodeFormat.Pdf417 => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatPdf417,
            BarcodeFormat.QR => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatQrCode,
            BarcodeFormat.Upce => Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.FormatUpcE,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };
    }
}