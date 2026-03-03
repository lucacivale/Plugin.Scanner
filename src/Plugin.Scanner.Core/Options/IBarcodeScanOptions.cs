using Plugin.Scanner.Core.Models.Enums;

namespace Plugin.Scanner.Core.Options;

/// <summary>
/// Defines configuration options for barcode scanning operations.
/// </summary>
public interface IBarcodeScanOptions : IScanOptions
{
    /// <summary>
    /// Gets the barcode formats to recognize during scanning.
    /// </summary>
    BarcodeFormat Formats { get; }

    /// <summary>
    /// Gets a value indicating whether multiple barcodes can be recognized simultaneously.
    /// </summary>
    bool RecognizeMultiple { get; }
}