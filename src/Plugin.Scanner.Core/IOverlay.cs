namespace Plugin.Scanner.Core;

/// <summary>
/// Defines an overlay component for scanner views.
/// </summary>
public partial interface IOverlay : IDisposable
{
    /// <summary>
    /// Adds the overlay UI elements to the scanner view.
    /// </summary>
    void AddOverlay();

    /// <summary>
    /// Adds a region of interest to restrict scanning to a specific area.
    /// </summary>
    /// <param name="regionOfInterest">The region of interest, or <c>null</c> to scan the entire view.</param>
    void AddRegionOfInterest(IRegionOfInterest? regionOfInterest);

    /// <summary>
    /// Cleans up overlay resources and removes event handlers.
    /// </summary>
    void Cleanup();
}