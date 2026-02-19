using CoreAnimation;
using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Views;

internal sealed class DataScannerRegionOfInterest : UIView
{
    private const float CornerRadius = 30f;

    private readonly IRegionOfInterest _regionOfInterest;

    private CAShapeLayer _baseLayer;
    private CAShapeLayer _highlightLayer;
    private CABasicAnimation? _strokeAnimation;

    public DataScannerRegionOfInterest(IRegionOfInterest regionOfInterest)
    {
        _regionOfInterest = regionOfInterest;

        BackgroundColor = UIColor.Clear;

        _baseLayer = new CAShapeLayer
        {
            StrokeColor = UIColor.Red.CGColor,
            LineWidth = 6f,
            FillColor = UIColor.Clear.CGColor,
        };

        _highlightLayer = new CAShapeLayer
        {
            StrokeColor = UIColor.Red.CGColor,
            LineWidth = 14f,
            LineCap = CAShapeLayer.CapRound,
            FillColor = UIColor.Clear.CGColor,
        };

        Layer.AddSublayer(_baseLayer);
        Layer.AddSublayer(_highlightLayer);
    }

    public void StartStrokeAnimation()
    {
        if (_highlightLayer.Path == null)
        {
            return;
        }

        _strokeAnimation = CABasicAnimation.FromKeyPath("strokeEnd");
        _strokeAnimation.From = NSNumber.FromInt32(0);
        _strokeAnimation.To = NSNumber.FromInt32(1);
        _strokeAnimation.Duration = 3.0;
        _strokeAnimation.RepeatCount = float.MaxValue;
        _strokeAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);

        _highlightLayer.AddAnimation(_strokeAnimation, "strokeAnimation");
    }

    public void StopStrokeAnimation()
    {
        _highlightLayer.RemoveAllAnimations();
    }

    public void Reset()
    {
        StopStrokeAnimation();
        SetupStroke();
        StartStrokeAnimation();
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        SetupStroke();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _baseLayer?.Dispose();
            _highlightLayer?.Dispose();
            _strokeAnimation?.Dispose();
        }

        base.Dispose(disposing);
    }

    private static nfloat GetPathLength(CGPath? path)
    {
        using UIBezierPath pathMeasure = new()
        {
            CGPath = path,
        };

        return pathMeasure.CGPath?.IsEmpty == true ? 0f : pathMeasure.Bounds.Width + pathMeasure.Bounds.Height;
    }

    private void SetupStroke()
    {
        CGRect regionRect = _regionOfInterest.CalculateRegionOfInterest().ToRect();
        UIBezierPath path = UIBezierPath.FromRoundedRect(regionRect, CornerRadius);

        _baseLayer.Path = path.CGPath;
        _highlightLayer.Path = path.CGPath;

        float pathLength = (float)GetPathLength(path.CGPath);

        _highlightLayer.LineDashPattern = [NSNumber.FromFloat(pathLength * 0.15f), NSNumber.FromFloat(pathLength)];
        _highlightLayer.LineDashPhase = 0;
    }
}
