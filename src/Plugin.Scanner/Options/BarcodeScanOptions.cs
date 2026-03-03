using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Models.Enums;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Overlays.Barcode;

namespace Plugin.Scanner.Options;

/// <summary>
/// Provides configuration options for barcode scanning operations.
/// </summary>
public sealed class BarcodeScanOptions : IBarcodeScanOptions
{
    /// <summary>
    /// Gets or sets the barcode formats to recognize during scanning.
    /// </summary>
    public BarcodeFormat Formats { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether multiple barcodes can be recognized simultaneously.
    /// </summary>
    public bool RecognizeMultiple { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether detected barcodes should be visually highlighted.
    /// </summary>
    public bool IsHighlightingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a pinch-to-zoom gesture is enabled during scanning.
    /// </summary>
    public bool IsPinchToZoomEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the region of interest that limits the scanning area, or <c>null</c> to scan the entire frame.
    /// </summary>
    public IRegionOfInterest? RegionOfInterest { get; set; }

    /// <summary>
    /// Gets or sets the overlay displayed during scanning operations.
    /// </summary>
    public IOverlay Overlay { get; set; } = new BarcodeScannerOverlay();

    /// <summary>
    /// Releases resources used by the scan options, including the overlay.
    /// </summary>
    public void Dispose()
    {
        Overlay.Dispose();
    }
}