using System.Drawing;
using Plugin.Scanner.Core;

namespace Plugin.Scanner;

/// <summary>
/// Represents a centered region of interest that can be defined by either fixed dimensions or a percentage of the scanning area.
/// </summary>
public sealed class CenteredRegionOfInterest : IRegionOfInterest
{
    private int _widthConstraint;
    private int _heightConstraint;
    private int _fixedWidth;
    private int _fixedHeight;
    private double _widthPercent;
    private double _heightPercent;
    private bool _isConfigured;
    private bool _fixedDimensions;
    private bool _percentageDimensions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CenteredRegionOfInterest"/> class.
    /// </summary>
    public CenteredRegionOfInterest()
    {
    }

    /// <summary>
    /// Sets the region of interest to use fixed width and height dimensions.
    /// </summary>
    /// <param name="width">The width of the region of interest in pixels.</param>
    /// <param name="height">The height of the region of interest in pixels.</param>
    /// <returns>The current instance for fluent configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when width or height is less than or equal to zero.</exception>
    public CenteredRegionOfInterest WithFixedDimensions(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentException("Width must be greater than zero.", nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentException("Height must be greater than zero.", nameof(height));
        }

        _fixedWidth = width;
        _fixedHeight = height;
        _fixedDimensions = true;
        _isConfigured = true;

        return this;
    }

    /// <summary>
    /// Sets the region of interest to use percentage-based dimensions relative to the scanning area.
    /// </summary>
    /// <param name="widthPercent">The width of the region of interest as a percentage (0-100) of the scanning area.</param>
    /// <param name="heightPercent">The height of the region of interest as a percentage (0-100) of the scanning area.</param>
    /// <returns>The current instance for fluent configuration.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when percentages are not between 0 and 100.</exception>
    public CenteredRegionOfInterest WithPercentageDimensions(double widthPercent, double heightPercent)
    {
        if (widthPercent <= 0 || widthPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(widthPercent), "Percentage must be between 0 and 100.");
        }

        if (heightPercent <= 0 || heightPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(heightPercent), "Percentage must be between 0 and 100.");
        }

        _widthPercent = widthPercent;
        _heightPercent = heightPercent;
        _percentageDimensions = true;
        _isConfigured = true;

        return this;
    }

    /// <summary>
    /// Sets the constraints for the scanning area dimensions.
    /// </summary>
    /// <param name="widthConstraint">The total width of the scanning area.</param>
    /// <param name="heightConstraint">The total height of the scanning area.</param>
    /// <exception cref="ArgumentException">Thrown when constraints are less than or equal to zero.</exception>
    public void SetConstraints(int widthConstraint, int heightConstraint)
    {
        _widthConstraint = widthConstraint;
        _heightConstraint = heightConstraint;
    }

    /// <summary>
    /// Calculates the centered region of interest within the scanning area.
    /// </summary>
    /// <returns>A rectangle representing the centered region of interest.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the region is not configured or constraints are not set.</exception>
    public Rectangle CalculateRegionOfInterest()
    {
        if (!_isConfigured)
        {
            throw new InvalidOperationException("Region of interest is not configured. Use WithFixedDimensions() or WithPercentageDimensions().");
        }

        if (_widthConstraint <= 0
            || _heightConstraint <= 0)
        {
            throw new InvalidOperationException("Constraints must be set before calculating the region of interest. Use SetConstraints().");
        }

        Rectangle rectangle = Rectangle.Empty;

        if (_fixedDimensions)
        {
            rectangle = new(
                (_widthConstraint / 2) - (_fixedWidth / 2),
                (_heightConstraint / 2) - (_fixedHeight / 2),
                _fixedWidth,
                _fixedHeight);
        }
        else if (_percentageDimensions)
        {
            int roiWidth = Convert.ToInt32(_widthConstraint * _widthPercent / 100);
            int roiHeight = Convert.ToInt32(_heightConstraint * _heightPercent / 100);

            int x = (_widthConstraint - roiWidth) / 2;
            int y = (_heightConstraint - roiHeight) / 2;

            rectangle = new Rectangle(
                x,
                y,
                roiWidth,
                roiHeight);
        }

        return rectangle;
    }
}