using CoreAnimation;
using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Views;

internal sealed class DataScannerRegionOfInterest : UIView
{
    private const float CornerRadius = 30f;
    private const float VisibleDashLength = 80f;

    private readonly IRegionOfInterest _regionOfInterest;

    private readonly CAShapeLayer _baseLayer;
    private readonly CAShapeLayer _highlightLayer;

    private CABasicAnimation? _strokeAnimation;
    private nfloat _pathLength;

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

    public void StopStrokeAnimation()
    {
        _highlightLayer.RemoveAllAnimations();
    }

    public void SetupStroke()
    {
        CGRect regionRect = _regionOfInterest.CalculateRegionOfInterest().ToRect();
        UIBezierPath path = UIBezierPath.FromRoundedRect(regionRect, CornerRadius);

        _baseLayer.Path = path.CGPath;
        _highlightLayer.Path = path.CGPath;

        _pathLength = CalculateRoundedRectPerimeter(regionRect, CornerRadius);
    }

    public void Reset()
    {
        StopStrokeAnimation();
        SetupStroke();
        StartStrokeAnimation();
    }

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

    private static nfloat CalculateRoundedRectPerimeter(CGRect rect, nfloat radius)
    {
        nfloat w = rect.Width;
        nfloat h = rect.Height;
        nfloat r = radius;

        return (2 * (w - (2 * r) + h - (2 * r))) + (2 * (nfloat)Math.PI * r);
    }
}