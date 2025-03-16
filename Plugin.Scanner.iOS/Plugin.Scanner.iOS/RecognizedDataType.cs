using Vision;

namespace Plugin.Scanner.iOS;

/// <summary>
/// A type of data that the scanner recognizes.
/// </summary>
public sealed class RecognizedDataType
{
    private readonly Plugin.Scanner.iOS.Binding.RecognizedDataType? _text;
    private readonly Plugin.Scanner.iOS.Binding.RecognizedDataType? _barcode;

    private RecognizedDataType(string[] languages, TextContentType textContentType)
    {
        _text = Plugin.Scanner.iOS.Binding.RecognizedDataType.Text(languages, textContentType.ToVnTextContentType());
    }

    private RecognizedDataType(VNBarcodeSymbology[] symbologies)
    {
        _barcode = Plugin.Scanner.iOS.Binding.RecognizedDataType.Barcode(symbologies.Select(x => x.ToString()).ToArray());
    }

    /// <summary>
    /// Creates a data type for text and information the scanner finds in text.
    /// </summary>
    /// <param name="languages">The identifiers for the languages that you want
    /// prioritized in the order of language processing. To specify a
    /// person’s preferred languages, pass an empty set. This parameter gives the scanner
    /// a hint on which language processing models to use. The scanner still recognizes all
    /// supported languages.</param>
    /// <param name="textContentType">The specific type of semantic text to find. To
    /// identify all content types, pass `<see cref="TextContentType.Default"/>.</param>
    /// <returns>A text data type.</returns>
    public static RecognizedDataType Text(string[]? languages = null, TextContentType textContentType = TextContentType.Default)
    {
        return new(languages ?? [], textContentType);
    }

    /// <summary>
    /// Creates a data type for barcodes the use the specified symbologies.
    /// </summary>
    /// <param name="symbologies">The barcode symbologies that the scanner recognizes.</param>
    /// <returns>A barcode data type for the specified symbologies.</returns>
    public static RecognizedDataType Barcode(VNBarcodeSymbology[] symbologies)
    {
        return new(symbologies);
    }

    /// <summary>
    /// Access native data type.
    /// </summary>
    /// <returns>Native data type.</returns>
    public Plugin.Scanner.iOS.Binding.RecognizedDataType? DataType()
    {
        Plugin.Scanner.iOS.Binding.RecognizedDataType? dataType = null;
        if (_text is not null)
        {
            dataType = _text;
        }

        if (_barcode is not null)
        {
            dataType = _barcode;
        }

        return dataType;
    }
}