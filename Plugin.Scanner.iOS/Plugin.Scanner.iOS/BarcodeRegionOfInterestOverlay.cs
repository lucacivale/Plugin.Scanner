#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

internal sealed class BarcodeRegionOfInterestOverlay : UIView
{
    private const float SizeMultiplier = 0.2f;
    private const float LineWidth = 10f;

    private readonly DataScannerViewController _dataScannerViewController;
    private readonly UIColor _lineColor = UIColor.FromRGB(206, 181, 61);
    private readonly UIBezierPath _path;

    private bool _isDisposed;

    public BarcodeRegionOfInterestOverlay(DataScannerViewController dataScannerViewController)
    {
        _dataScannerViewController = dataScannerViewController;
        _path = new UIBezierPath();
        BackgroundColor = UIColor.Clear;
    }

    public override void Draw(CGRect rect)
    {
        base.Draw(rect);

        DrawCorners();

        _dataScannerViewController.RegionOfInterest = rect;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (disposing)
        {
            _lineColor.Dispose();
            _path.Dispose();
        }
    }

    private void DrawCorners()
    {
        _path.RemoveAllPoints();

        _path.LineWidth = LineWidth;

        _lineColor.SetStroke();

        _path.MoveTo(new CGPoint(0, 0));
        _path.AddLineTo(new CGPoint(x: Bounds.Size.Width * SizeMultiplier, 0));
        _path.Stroke();

        _path.MoveTo(new CGPoint(Bounds.Size.Width - (Bounds.Size.Width * SizeMultiplier), 0));
        _path.AddLineTo(new CGPoint(Bounds.Size.Width, 0));
        _path.AddLineTo(new CGPoint(Bounds.Size.Width, Bounds.Size.Height * SizeMultiplier));
        _path.Stroke();

        _path.MoveTo(new CGPoint(Bounds.Size.Width, Bounds.Size.Height - (Bounds.Size.Height * SizeMultiplier)));
        _path.AddLineTo(new CGPoint(Bounds.Size.Width, Bounds.Size.Height));
        _path.AddLineTo(new CGPoint(Bounds.Size.Width - (Bounds.Size.Width * SizeMultiplier), Bounds.Size.Height));
        _path.Stroke();

        _path.MoveTo(new CGPoint(Bounds.Size.Width * SizeMultiplier, Bounds.Size.Height));
        _path.AddLineTo(new CGPoint(0, Bounds.Size.Height));
        _path.AddLineTo(new CGPoint(0, Bounds.Size.Height - (Bounds.Size.Height * SizeMultiplier)));
        _path.Stroke();

        _path.MoveTo(new CGPoint(0, Bounds.Size.Height * SizeMultiplier));
        _path.AddLineTo(new CGPoint(0, 0));
        _path.Stroke();
    }
}