namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Defines configuration options for barcode scanning operations.
/// </summary>
/// <remarks>
/// <para>
/// Implementations of this interface specify which barcode formats should be recognized
/// during a scanning operation. The default implementation is <see cref="BarcodeScanOptions"/>.
/// </para>
/// <para>
/// Configure this interface before passing it to <see cref="IBarcodeScanner.ScanAsync"/> to
/// control which barcode types the scanner will recognize.
/// </para>
/// </remarks>
/// <seealso cref="BarcodeScanOptions"/>
/// <seealso cref="IBarcodeScanner"/>
public interface IBarcodeScanOptions
{
    /// <summary>
    /// Gets the barcode formats to be recognized during scanning.
    /// </summary>
    /// <value>
    /// A <see cref="BarcodeFormat"/> value representing one or more barcode formats.
    /// Multiple formats can be combined using the bitwise OR operator, or use 
    /// <see cref="BarcodeFormat.All"/> to recognize all supported formats.
    /// </value>
    /// <remarks>
    /// <para>
    /// Limiting the recognized formats to only those needed can improve scanning
    /// performance and reduce false positives.
    /// </para>
    /// <para>
    /// Common combinations include:
    /// <list type="bullet">
    /// <item><description><see cref="BarcodeFormat.QR"/> - For QR codes only</description></item>
    /// <item><description><see cref="BarcodeFormat.Ean13"/> | <see cref="BarcodeFormat.Ean8"/> - For retail product codes</description></item>
    /// <item><description><see cref="BarcodeFormat.All"/> - For maximum compatibility</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    BarcodeFormat Formats { get; }
}