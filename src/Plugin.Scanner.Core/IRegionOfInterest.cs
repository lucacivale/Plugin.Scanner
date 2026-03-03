using System.Drawing;

namespace Plugin.Scanner.Core;

/// <summary>
/// Defines a region of interest for restricting scanning to a specific area.
/// </summary>
public interface IRegionOfInterest
{
    /// <summary>
    /// Sets the available width and height constraints for calculating the region.
    /// </summary>
    /// <param name="widthConstraint">The available width.</param>
    /// <param name="heightConstraint">The available height.</param>
    void SetConstraints(int widthConstraint, int heightConstraint);

    /// <summary>
    /// Calculates and returns the region of interest rectangle.
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> representing the region of interest.</returns>
    Rectangle CalculateRegionOfInterest();
}
