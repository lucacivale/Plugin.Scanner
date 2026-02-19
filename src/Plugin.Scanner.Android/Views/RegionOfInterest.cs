using Android.Animation;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using ALinearInterpolator = Android.Views.Animations.LinearInterpolator;
using APath = Android.Graphics.Path;

namespace Plugin.Scanner.Android.Views;

internal sealed class RegionOfInterest : View
{
    private const float RornerRadius = 30f;

    private readonly Paint _basePaint;
    private readonly Paint _highlightPaint;
    private readonly ALinearInterpolator _linearInterpolator;
    private readonly IRegionOfInterest _regionOfInterest;

    private float _pathLength;
    private APath? _path;
    private RectF? _rect;
    private float _phase;
    private ValueAnimator? _animator;

    public RegionOfInterest(Context? cotnext, IRegionOfInterest regionOfInterest)
        : base(cotnext)
    {
        _regionOfInterest = regionOfInterest;

        _basePaint = new Paint(PaintFlags.AntiAlias)
        {
            Color = Color.Red,
            StrokeWidth = 6f,
        };
        _basePaint.SetStyle(Paint.Style.Stroke);
        _basePaint.StrokeWidth = 6f;

        _highlightPaint = new Paint(PaintFlags.AntiAlias)
        {
            Color = Color.Red,
            StrokeWidth = 14f,
            StrokeCap = Paint.Cap.Round,
        };
        _highlightPaint.SetStyle(Paint.Style.Stroke);

        _linearInterpolator = new();
    }

    public void StartStrokeAnimation()
    {
        _animator = ValueAnimator.OfFloat(0, _pathLength);
        _animator?.SetDuration(3000);
        _animator?.RepeatCount = ValueAnimator.Infinite;
        _animator?.RepeatMode = ValueAnimatorRepeatMode.Restart;
        _animator?.SetInterpolator(_linearInterpolator);

        _animator?.Update += (s, e) =>
        {
            _phase = (float)e.Animation.AnimatedValue!;
            Invalidate();
        };

        _animator?.Start();
    }

    public void StopStrokeAnimation()
    {
        _animator?.Cancel();
        _highlightPaint.SetPathEffect(null);
        Invalidate();
    }

    public void Reset()
    {
        StopStrokeAnimation();

        InitStroke();

        StartStrokeAnimation();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopStrokeAnimation();

            _basePaint.Dispose();
            _highlightPaint.Dispose();
            _rect?.Dispose();
            _path?.Dispose();
            _linearInterpolator.Dispose();

            _animator?.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void OnAttachedToWindow()
    {
        InitStroke();

        base.OnAttachedToWindow();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        if (_path is null)
        {
            return;
        }

        canvas.DrawPath(_path, _basePaint);

        float visibleLength = _pathLength * 0.15f;

        using DashPathEffect dashPathEffect = new([visibleLength, _pathLength], _phase);

        _highlightPaint.SetPathEffect(dashPathEffect);

        canvas.DrawPath(_path, _highlightPaint);
    }

    private void InitStroke()
    {
        if (Context is null)
        {
            return;
        }

        using Rect rect = _regionOfInterest.CalculateRegionOfInterest().ToRect(Context);
        _rect = new(rect);

        if (APath.Direction.Cw is APath.Direction cw)
        {
            _path = new();
            _path.AddRoundRect(_rect, RornerRadius, RornerRadius, cw);
            _pathLength = new PathMeasure(_path, true).Length;
        }
    }
}
