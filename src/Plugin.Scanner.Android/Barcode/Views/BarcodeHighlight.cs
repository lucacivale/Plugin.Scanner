namespace Plugin.Scanner.Android.Barcode.Views;

internal sealed class BarcodeHighlight : Drawable
{
    private readonly Paint _boxPaint;
    private readonly Rect _box;

    public BarcodeHighlight(Rect box)
    {
        _boxPaint = new Paint()
        {
            Color = Color.Yellow,
            StrokeWidth = 5F,
            Alpha = 200,
        };
        _boxPaint.SetStyle(Paint.Style.Stroke);

        _box = new Rect(box);
        _box.Inset(-30, -30);
    }

    public override void Draw(Canvas canvas)
    {
        canvas.DrawRect(_box, _boxPaint);
    }

    public override void SetAlpha(int alpha)
    {
        _boxPaint.Alpha = alpha;
    }

    public override void SetColorFilter(ColorFilter colorFilter)
    {
        _boxPaint.SetColorFilter(colorFilter);
    }

    public override int Opacity => (int)Format.Translucent;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _boxPaint.Dispose();
        }

        base.Dispose(disposing);
    }
}
