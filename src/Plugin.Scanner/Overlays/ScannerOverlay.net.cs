using Plugin.Scanner.Core;

namespace Plugin.Scanner.Overlays;

/// <summary>
/// Provides default scanner overlay implementation for non-mobile platforms.
/// </summary>
internal abstract partial class ScannerOverlay
{
    /// <summary>
    /// Releases all resources used by the scanner overlay.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Adds the overlay to the scanner view.
    /// </summary>
    public void AddOverlay()
    {
    }

    /// <summary>
    /// Configures the region of interest for the scanner overlay.
    /// </summary>
    /// <param name="regionOfInterest">The region to focus scanning on, or null to scan the entire view.</param>
    public void AddRegionOfInterest(IRegionOfInterest? regionOfInterest)
    {
    }

    /// <summary>
    /// Removes the overlay and cleans up associated resources.
    /// </summary>
    public void Cleanup()
    {
    }

    /// <summary>
    /// Releases the unmanaged resources used by the overlay and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
