using CoreAnimation;
using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.Views.iOS;

/// <summary>
/// Displays an animated border around the scanner's region of interest on iOS.
/// </summary>
internal sealed class DataScannerRegionOfInterest : UIView
{
    private const float CornerRadius = 30f;
    private const float VisibleDashLength = 80f;

    private readonly IRegionOfInterest _regionOfInterest;

    private readonly CAShapeLayer _baseLayer;
    private readonly CAShapeLayer _highlightLayer;

    private CABasicAnimation? _strokeAnimation;
    private nfloat _pathLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerRegionOfInterest"/> class.
    /// </summary>
    /// <param name="regionOfInterest">The region of interest configuration.</param>
    public DataScannerRegionOfInterest(IRegionOfInterest regionOfInterest)
    {
        _regionOfInterest = regionOfInterest;

        BackgroundColor = UIColor.Clear;

        _baseLayer = new CAShapeLayer
        {
            StrokeColor = UIColor.Red.CGColor,
            LineWidth = 4f,
            FillColor = UIColor.Clear.CGColor,
        };

        _highlightLayer = new CAShapeLayer
        {
            StrokeColor = UIColor.Red.CGColor,
            LineWidth = 8f,
            LineCap = CAShapeLayer.CapRound,
            FillColor = UIColor.Clear.CGColor,
        };

        Layer.AddSublayer(_baseLayer);
        Layer.AddSublayer(_highlightLayer);
    }

    /// <inheritdoc/>
    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        Reset();
    }

    /// <summary>
    /// Starts the animated highlight stroke around the border.
    /// </summary>
    public void StartStrokeAnimation()
    {
        if (_highlightLayer.Path is null
            || _pathLength <= 0)
        {
            return;
        }

        StopStrokeAnimation();

        nfloat gapLength = _pathLength - VisibleDashLength;

        _highlightLayer.LineDashPattern =
        [
            NSNumber.FromNFloat(VisibleDashLength),
            NSNumber.FromNFloat(gapLength)
        ];

        _strokeAnimation = CABasicAnimation.FromKeyPath("lineDashPhase");
        _strokeAnimation.By = NSNumber.FromNFloat(_pathLength);
        _strokeAnimation.Duration = 2.5;
        _strokeAnimation.RepeatCount = float.MaxValue;
        _strokeAnimation.Cumulative = true;
        _strokeAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
        _strokeAnimation.RemovedOnCompletion = false;

        _highlightLayer.AddAnimation(_strokeAnimation, "dashPhase");
    }

    /// <summary>
    /// Stops the stroke animation.
    /// </summary>
    public void StopStrokeAnimation()
    {
        _highlightLayer.RemoveAllAnimations();
    }

    /// <summary>
    /// Initializes the border path for the region of interest.
    /// </summary>
    public void SetupStroke()
    {
        CGRect regionRect = _regionOfInterest.CalculateRegionOfInterest().ToRect();
        UIBezierPath path = UIBezierPath.FromRoundedRect(regionRect, CornerRadius);

        _baseLayer.Path = path.CGPath;
        _highlightLayer.Path = path.CGPath;

        _pathLength = CalculateRoundedRectPerimeter(regionRect, CornerRadius);
    }

    /// <summary>
    /// Resets and restarts the stroke animation.
    /// </summary>
    public void Reset()
    {
        StopStrokeAnimation();
        SetupStroke();
        StartStrokeAnimation();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopStrokeAnimation();
            _baseLayer.Dispose();
            _highlightLayer.Dispose();
            _strokeAnimation?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Calculates the perimeter of a rounded rectangle.
    /// </summary>
    /// <param name="rect">The rectangle bounds.</param>
    /// <param name="radius">The corner radius.</param>
    /// <returns>The total perimeter length.</returns>
    private static nfloat CalculateRoundedRectPerimeter(CGRect rect, nfloat radius)
    {
        nfloat w = rect.Width;
        nfloat h = rect.Height;
        nfloat r = radius;

        return (2 * (w - (2 * r) + h - (2 * r))) + (2 * (nfloat)Math.PI * r);
    }
}