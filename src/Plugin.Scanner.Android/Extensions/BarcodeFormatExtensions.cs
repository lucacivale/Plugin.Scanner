using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="BarcodeFormat"/> flags to ML Kit barcode format identifiers.
/// </summary>
internal static class BarcodeFormatExtensions
{
    /// <summary>
    /// Converts the barcode format flags to a collection of ML Kit barcode format identifiers.
    /// </summary>
    /// <param name="formats">The barcode format flags to convert.</param>
    /// <returns>A collection of ML Kit barcode format integer identifiers.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported barcode format is encountered.</exception>
    public static IEnumerable<int> ToBarcodeFormats(this BarcodeFormat formats)
    {
        Func<BarcodeFormat, int> select = flag => flag switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null),
        };

        IEnumerable<int> symbologies;

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

        return symbologies;
    }
}