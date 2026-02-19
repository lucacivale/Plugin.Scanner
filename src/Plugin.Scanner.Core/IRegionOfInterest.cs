using System.Drawing;

namespace Plugin.Scanner.Core;

public interface IRegionOfInterest
{
    void SetConstraints(int widthConstraint, int heightConstraint);

    Rectangle CalculateRegionOfInterest();
}
