namespace Plugin.Scanner.Views.Android;

internal class TextBlockHighlight : Drawable
{
    private readonly Rect _rect;
    private readonly Paint _paint;

    public TextBlockHighlight(Rect rect, Color baseColor, float hueOffsetDegrees = 0f)
    {
        _rect = rect;

        _paint = new Paint
        {
            AntiAlias = true
        };

        float[] hsv = new float[3];
        Color.ColorToHSV(baseColor, hsv);

        hsv[0] = (hsv[0] + hueOffsetDegrees) % 360f;

        _paint.Color = Color.HSVToColor(120, hsv);
    }

    public override void Draw(Canvas canvas)
    {
        if (_rect != null)
        {
            canvas.DrawRect(_rect, _paint);
        }
    }

    public override void SetAlpha(int alpha)
    {
        _paint.Alpha = alpha;
    }

    public override void SetColorFilter(ColorFilter? colorFilter)
    {
        _paint.SetColorFilter(colorFilter);
    }

    public override int Opacity => (int)Format.Translucent;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _paint.Dispose();
            _rect.Dispose();
        }

        base.Dispose(disposing);
    }
}
