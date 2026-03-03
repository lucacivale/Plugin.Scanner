using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Overlays.Text;

namespace Plugin.Scanner.Options;

/// <summary>
/// Provides configuration options for text scanning (OCR) operations.
/// </summary>
public sealed class TextScanOptions : ITextScanOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether detected text should be visually highlighted.
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
    public IOverlay Overlay { get; set; } = new TextScannerOverlay();

    /// <summary>
    /// Releases resources used by the scan options, including the overlay.
    /// </summary>
    public void Dispose()
    {
        Overlay.Dispose();
    }
}