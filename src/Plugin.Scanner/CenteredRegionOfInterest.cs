using System.Drawing;
using Plugin.Scanner.Core;

namespace Plugin.Scanner;

/// <summary>
/// Represents a region of interest that is centered within the scanning area.
/// </summary>
public sealed class CenteredRegionOfInterest : IRegionOfInterest
{
    private readonly int _width;
    private readonly int _height;

    private int _widthConstraint;
    private int _heightConstraint;

    /// <summary>
    /// Initializes a new instance of the <see cref="CenteredRegionOfInterest"/> class.
    /// </summary>
    /// <param name="width">The width of the region of interest.</param>
    /// <param name="height">The height of the region of interest.</param>
    public CenteredRegionOfInterest(int width, int height)
    {
        _width = width;
        _height = height;
    }

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
        return new(
            (_widthConstraint / 2) - (_width / 2),
            (_heightConstraint / 2) - (_height / 2),
            _width,
            _height);
    }
}
