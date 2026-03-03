using Plugin.Scanner.Core;

namespace Plugin.Scanner.Overlays.Barcode;

/// <summary>
/// Provides default barcode scanner overlay implementation for non-mobile platforms.
/// </summary>
internal sealed partial class BarcodeScannerOverlay
{
    /// <summary>
    /// Adds the overlay UI elements to the scanner view.
    /// </summary>
    public void AddOverlay()
    {
    }

    /// <summary>
    /// Adds a region of interest to restrict scanning to a specific area.
    /// </summary>
    /// <param name="regionOfInterest">The region of interest, or <c>null</c> to scan the entire view.</param>
    public void AddRegionOfInterest(IRegionOfInterest? regionOfInterest)
    {
    }

    /// <summary>
    /// Cleans up overlay resources and removes event handlers.
    /// </summary>
    public void Cleanup()
    {
    }

    /// <summary>
    /// Releases resources used by the overlay.
    /// </summary>
    public void Dispose()
    {
    }
}