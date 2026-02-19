using Plugin.Scanner.Core;
using System.Drawing;

namespace Plugin.Scanner.Models;

public class CenteredRegionOfInterest : IRegionOfInterest
{
    private readonly int _width;
    private readonly int _height;

    private int _widthConstraint;
    private int _heightConstraint;

    public CenteredRegionOfInterest(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void SetConstraints(int widthConstraint, int heightConstraint)
    {
        _widthConstraint = widthConstraint;
        _heightConstraint = heightConstraint;
    }

    public Rectangle CalculateRegionOfInterest()
    {
        return new(
            (_widthConstraint / 2) - (_width / 2),
            (_heightConstraint / 2) - (_height / 2),
            _width,
            _height);
    }
}
