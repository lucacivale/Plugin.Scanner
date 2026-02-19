using Android.Animation;
using Android.Views.Animations;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core;
using APath = Android.Graphics.Path;

namespace Plugin.Scanner.Android.Views;

internal sealed class RegionOfInterest : View
{
    private const float CornerRadius = 30f;
    private const float StrokeWidth = 12f;
    private const float HighlightWidth = 24f;
    private const float VisiblePercent = 0.10f;

    private readonly Paint _basePaint;
    private readonly Paint _highlightPaint;
    private readonly LinearInterpolator _linearInterpolator;
    private readonly IRegionOfInterest _regionOfInterest;

    private APath? _path;
    private APath? _segmentPath;
    private PathMeasure? _pathMeasure;

    private float _pathLength;
    private float _phase;
    private ValueAnimator? _animator;

    public RegionOfInterest(Context? context, IRegionOfInterest regionOfInterest)
        : base(context)
    {
        _regionOfInterest = regionOfInterest;

        _basePaint = new Paint(PaintFlags.AntiAlias)
        {
            Color = Color.Red,
            StrokeWidth = StrokeWidth,
        };
        _basePaint.SetStyle(Paint.Style.Stroke);

        _highlightPaint = new Paint(PaintFlags.AntiAlias)
        {
            Color = Color.Red,
            StrokeWidth = HighlightWidth,
            StrokeCap = Paint.Cap.Round,
        };
        _highlightPaint.SetStyle(Paint.Style.Stroke);

        _linearInterpolator = new LinearInterpolator();
    }

    public void StartStrokeAnimation()
    {
        if (_pathLength <= 0)
        {
            return;
        }

        _animator = ValueAnimator.OfFloat(0f, _pathLength);
        _animator?.SetDuration(2500);
        _animator?.RepeatCount = ValueAnimator.Infinite;
        _animator?.RepeatMode = ValueAnimatorRepeatMode.Restart;
        _animator?.SetInterpolator(_linearInterpolator);

        _animator?.Update += (_, e) =>
        {
            _phase = (float)e.Animation!.AnimatedValue!;
            Invalidate();
        };

        _animator?.Start();
    }

    public void StopStrokeAnimation()
    {
        _animator?.Cancel();
        _animator?.Dispose();
        _animator = null;
    }

    public void Reset()
    {
        StopStrokeAnimation();
        InitStroke();
        StartStrokeAnimation();
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        InitStroke();
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        if (_path is null
            || _pathMeasure is null)
        {
            return;
        }

        // Draw base border
        canvas.DrawPath(_path, _basePaint);

        float segmentLength = _pathLength * VisiblePercent;
        float start = _phase;
        float end = start + segmentLength;

        _segmentPath ??= new APath();
        _segmentPath.Reset();

        if (end <= _pathLength)
        {
            _pathMeasure.GetSegment(start, end, _segmentPath, true);
        }
        else
        {
            // Wrap around
            _pathMeasure.GetSegment(start, _pathLength, _segmentPath, true);
            _pathMeasure.GetSegment(0, end - _pathLength, _segmentPath, true);
        }

        canvas.DrawPath(_segmentPath, _highlightPaint);
    }

    private void InitStroke()
    {
        if (Context is null)
        {
            return;
        }

        using Rect rect = _regionOfInterest
            .CalculateRegionOfInterest()
            .ToRect(Context);

        using RectF rectF = new(rect);

        _path?.Dispose();
        _path = new APath();
        _path.AddRoundRect(rectF, CornerRadius, CornerRadius, APath.Direction.Ccw!);

        _pathMeasure?.Dispose();
        _pathMeasure = new PathMeasure(_path, true);
        _pathLength = _pathMeasure.Length;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopStrokeAnimation();

            _basePaint.Dispose();
            _highlightPaint.Dispose();
            _path?.Dispose();
            _segmentPath?.Dispose();
            _pathMeasure?.Dispose();
            _linearInterpolator.Dispose();
        }

        base.Dispose(disposing);
    }
}