using System.Drawing;
using Plugin.Scanner.Core;

namespace Plugin.Scanner;

/// <summary>
/// Represents a region of interest that is centered within the scanning area.
/// </summary>
public sealed class CenteredPopupRegionOfInterest : IRegionOfInterest
{
    private int _widthConstraint;
    private int _heightConstraint;

    /// <summary>
    /// Sets the constraints for the scanning area dimensions.
    /// </summary>
    /// <param name="widthConstraint">The total width of the scanning area.</param>
    /// <param name="heightConstraint">The total height of the scanning area.</param>
    public void SetConstraints(int widthConstraint, int heightConstraint)
    {
        _widthConstraint = widthConstraint;
        _heightConstraint = heightConstraint;
    }

    /// <summary>
    /// Calculates the centered region of interest within the scanning area.
    /// </summary>
    /// <returns>A rectangle representing the centered region of interest.</returns>
    public Rectangle CalculateRegionOfInterest()
    {
        int roiWidth = _widthConstraint / 2;
        int roiHeight = _heightConstraint / 2;

        int x = (_widthConstraint - roiWidth) / 2;
        int y = (_heightConstraint - roiHeight) / 2;

        return new Rectangle(
            x,
            y,
            roiWidth,
            roiHeight);
    }
}
