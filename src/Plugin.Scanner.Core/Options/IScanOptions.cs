namespace Plugin.Scanner.Core.Options;

/// <summary>
/// Defines common configuration options for scanning operations.
/// </summary>
public interface IScanOptions : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether detected items should be visually highlighted.
    /// </summary>
    bool IsHighlightingEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether a pinch-to-zoom gesture is enabled during scanning.
    /// </summary>
    bool IsPinchToZoomEnabled { get; }

    /// <summary>
    /// Gets the region of interest that limits the scanning area, or <c>null</c> to scan the entire frame.
    /// </summary>
    IRegionOfInterest? RegionOfInterest { get; }

    /// <summary>
    /// Gets the overlay displayed during scanning operations.
    /// </summary>
    IOverlay Overlay { get; }
}