using Android.Animation;
using Path = Android.Graphics.Path;

namespace Plugin.Scanner.Android.Views;

internal sealed class RecognizedItemHighlight : Drawable
{
    private const float CornerRadius = 25f;

    private const float CornerLength = 60f;

    private readonly RecognizedItem _item;
    private readonly Paint _boxPaint;
    private readonly Rect _box;

    public RecognizedItemHighlight(RecognizedItem item)
    {
        _item = item;

        _boxPaint = new Paint
        {
            Color = Color.Yellow,
            StrokeWidth = 10f,
            AntiAlias = true,
            Alpha = 200,
        };
        _boxPaint.SetStyle(Paint.Style.Stroke);

        _box = new Rect(_item.Bounds);
        _box.Inset(-30, -30);
    }

    public override int Opacity => (int)Format.Translucent;

    public RecognizedItem RecognizedItem => _item;

    public override void Draw(Canvas canvas)
    {
        using Path path = new();

        float l = _box.Left;
        float t = _box.Top;
        float r = _box.Right;
        float b = _box.Bottom;

        using RectF topLeftRect = new(l, t, l + (2 * CornerRadius), t + (2 * CornerRadius));
        using RectF topRightRect = new(r - (2 * CornerRadius), t, r, t + (2 * CornerRadius));
        using RectF bottomLeftRect = new(l, b - (2 * CornerRadius), l + (2 * CornerRadius), b);
        using RectF bottomRightRect = new(r - (2 * CornerRadius), b - (2 * CornerRadius), r, b);

        path.MoveTo(l + CornerRadius, t);
        path.LineTo(l + CornerLength, t);
        path.MoveTo(l, t + CornerRadius);
        path.LineTo(l, t + CornerLength);
        path.AddArc(topLeftRect, 180, 90);

        path.MoveTo(r - CornerRadius, t);
        path.LineTo(r - CornerLength, t);
        path.MoveTo(r, t + CornerRadius);
        path.LineTo(r, t + CornerLength);
        path.AddArc(topRightRect, 270, 90);

        path.MoveTo(r - CornerRadius, b);
        path.LineTo(r - CornerLength, b);
        path.MoveTo(r, b - CornerRadius);
        path.LineTo(r, b - CornerLength);
        path.AddArc(bottomRightRect, 0, 90);

        path.MoveTo(l + CornerRadius, b);
        path.LineTo(l + CornerLength, b);
        path.MoveTo(l, b - CornerRadius);
        path.LineTo(l, b - CornerLength);
        path.AddArc(bottomLeftRect, 90, 90);

        canvas.DrawPath(path, _boxPaint);
    }

    public override void SetAlpha(int alpha)
    {
        _boxPaint.Alpha = alpha;
    }

    public override void SetColorFilter(ColorFilter? colorFilter)
    {
        _boxPaint.SetColorFilter(colorFilter);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _boxPaint.Dispose();
            _box.Dispose();
        }

        base.Dispose(disposing);
    }
}
