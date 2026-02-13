using Path = Android.Graphics.Path;

namespace Plugin.Scanner.Android.Barcode.Views;

/// <summary>
/// Represents a custom drawable that renders a rounded corner highlight box around a detected barcode.
/// </summary>
internal sealed class BarcodeHighlight : Drawable
{
    /// <summary>
    /// The radius for the rounded corners of the highlight box.
    /// </summary>
    private const float CornerRadius = 25f;

    /// <summary>
    /// The length of each corner line extending from the corner arc.
    /// </summary>
    private const float CornerLength = 60f;

    private readonly Paint _boxPaint;
    private readonly Rect _box;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeHighlight"/> class.
    /// </summary>
    /// <param name="box">The bounding rectangle of the detected barcode in screen coordinates.</param>
    public BarcodeHighlight(Rect box)
    {
        _boxPaint = new Paint
        {
            Color = Color.Yellow,
            StrokeWidth = 10f,
            AntiAlias = true,
            Alpha = 200,
        };
        _boxPaint.SetStyle(Paint.Style.Stroke);

        _box = new Rect(box);
        _box.Inset(-30, -30);
    }

    /// <summary>
    /// Gets the opacity classification of this drawable.
    /// </summary>
    /// <value>
    /// Always returns <see cref="Format.Translucent"/> to indicate the drawable has transparency.
    /// </value>
    public override int Opacity => (int)Format.Translucent;

    /// <summary>
    /// Draws the barcode highlight overlay on the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas on which to draw the highlight.</param>
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

    /// <summary>
    /// Sets the alpha (opacity) value for the highlight.
    /// </summary>
    /// <param name="alpha">The alpha value (0-255), where 0 is fully transparent and 255 is fully opaque.</param>
    public override void SetAlpha(int alpha)
    {
        _boxPaint.Alpha = alpha;
    }

    /// <summary>
    /// Sets a color filter for the highlight paint.
    /// </summary>
    /// <param name="colorFilter">The color filter to apply, or <c>null</c> to remove any existing filter.</param>
    public override void SetColorFilter(ColorFilter? colorFilter)
    {
        _boxPaint.SetColorFilter(colorFilter);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BarcodeHighlight"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
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
