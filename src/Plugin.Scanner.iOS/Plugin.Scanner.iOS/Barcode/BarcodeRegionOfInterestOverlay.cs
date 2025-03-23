#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <inheritdoc />
internal sealed class BarcodeRegionOfInterestOverlay : UIView
{
    private const float SizeMultiplier = 0.2f;
    private const float LineWidth = 10f;

    private readonly DataScannerViewController _dataScannerViewController;
    private readonly UIColor _lineColor = UIColor.FromRGB(206, 181, 61);
    private readonly UIBezierPath _path;

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeRegionOfInterestOverlay"/> class.
    /// </summary>
    /// <param name="dataScannerViewController">Data scanner to add overlay.</param>
    public BarcodeRegionOfInterestOverlay(DataScannerViewController dataScannerViewController)
    {
        this._dataScannerViewController = dataScannerViewController;
        this._path = new UIBezierPath();
        this.BackgroundColor = UIColor.Clear;
    }

    /// <inheritdoc/>
    public override void Draw(CGRect rect)
    {
        base.Draw(rect);

        this.DrawCorners();

        this._dataScannerViewController.RegionOfInterest = rect;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (this._isDisposed)
        {
            return;
        }

        this._isDisposed = true;

        if (disposing)
        {
            this._lineColor.Dispose();
            this._path.Dispose();
        }
    }

    private void DrawCorners()
    {
        this._path.RemoveAllPoints();

        this._path.LineWidth = LineWidth;

        this._lineColor.SetStroke();

        this._path.MoveTo(new CGPoint(0, 0));
        this._path.AddLineTo(new CGPoint(x: this.Bounds.Size.Width * SizeMultiplier, 0));
        this._path.Stroke();

        this._path.MoveTo(new CGPoint(this.Bounds.Size.Width - (this.Bounds.Size.Width * SizeMultiplier), 0));
        this._path.AddLineTo(new CGPoint(this.Bounds.Size.Width, 0));
        this._path.AddLineTo(new CGPoint(this.Bounds.Size.Width, this.Bounds.Size.Height * SizeMultiplier));
        this._path.Stroke();

        this._path.MoveTo(new CGPoint(this.Bounds.Size.Width, this.Bounds.Size.Height - (this.Bounds.Size.Height * SizeMultiplier)));
        this._path.AddLineTo(new CGPoint(this.Bounds.Size.Width, this.Bounds.Size.Height));
        this._path.AddLineTo(new CGPoint(this.Bounds.Size.Width - (this.Bounds.Size.Width * SizeMultiplier), this.Bounds.Size.Height));
        this._path.Stroke();

        this._path.MoveTo(new CGPoint(this.Bounds.Size.Width * SizeMultiplier, this.Bounds.Size.Height));
        this._path.AddLineTo(new CGPoint(0, this.Bounds.Size.Height));
        this._path.AddLineTo(new CGPoint(0, this.Bounds.Size.Height - (this.Bounds.Size.Height * SizeMultiplier)));
        this._path.Stroke();

        this._path.MoveTo(new CGPoint(0, this.Bounds.Size.Height * SizeMultiplier));
        this._path.AddLineTo(new CGPoint(0, 0));
        this._path.Stroke();
    }
}