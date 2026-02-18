namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Provides configuration options for barcode scanning operations.
/// </summary>
/// <remarks>
/// This is the default implementation of <see cref="IBarcodeScanOptions"/>.
/// The class is sealed and mutable, allowing configuration of scan options before initiating a scan.
/// </remarks>
public sealed class BarcodeScanOptions : IBarcodeScanOptions
{
    /// <summary>
    /// Gets or sets the barcode formats to be recognized during scanning.
    /// </summary>
    /// <value>
    /// A <see cref="BarcodeFormat"/> value representing one or more barcode formats.
    /// Multiple formats can be combined using the bitwise OR operator.
    /// </value>
    /// <remarks>
    /// <para>
    /// Use <see cref="BarcodeFormat.All"/> to recognize all supported formats, or combine specific formats for targeted scanning.
    /// </para>
    /// <example>
    /// Scan for QR codes and EAN-13 barcodes:
    /// <code>
    /// var options = new BarcodeScanOptions
    /// {
    ///     Formats = BarcodeFormat.QR | BarcodeFormat.Ean13
    /// };
    /// </code>
    /// </example>
    /// </remarks>
    public BarcodeFormat Formats { get; set; }

    public bool RecognizeMultiple { get; set; }

    public bool IsHighlightingEnabled { get; set; } = true;

    public bool IsPinchToZoomEnabled { get; set; } = true;
}