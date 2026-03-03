namespace Plugin.Scanner.Views.Android;

/// <summary>
/// Draws a semi-transparent colored highlight overlay for recognized text blocks.
/// </summary>
internal sealed class TextBlockHighlight : Drawable
{
    private readonly Rect _rect;
    private readonly Paint _paint;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockHighlight"/> class.
    /// </summary>
    /// <param name="rect">The rectangular bounds of the text block.</param>
    /// <param name="baseColor">The base color for the highlight.</param>
    /// <param name="hueOffsetDegrees">Optional hue offset in degrees to vary the color.</param>
    public TextBlockHighlight(Rect rect, Color baseColor, float hueOffsetDegrees = 0f)
    {
        _rect = rect;

        _paint = new Paint
        {
            AntiAlias = true,
        };

        float[] hsv = new float[3];
        Color.ColorToHSV(baseColor, hsv);

        hsv[0] = (hsv[0] + hueOffsetDegrees) % 360f;

        _paint.Color = Color.HSVToColor(120, hsv);
    }

    /// <summary>
    /// Gets the opacity of the drawable.
    /// </summary>
    public override int Opacity => (int)Format.Translucent;

    /// <summary>
    /// Draws the highlight rectangle on the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    public override void Draw(Canvas canvas)
    {
        canvas.DrawRect(_rect, _paint);
    }

    /// <summary>
    /// Sets the alpha transparency value.
    /// </summary>
    /// <param name="alpha">The alpha value (0-255).</param>
    public override void SetAlpha(int alpha)
    {
        _paint.Alpha = alpha;
    }

    /// <summary>
    /// Sets the color filter for the paint.
    /// </summary>
    /// <param name="colorFilter">The color filter to apply.</param>
    public override void SetColorFilter(ColorFilter? colorFilter)
    {
        _paint.SetColorFilter(colorFilter);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="TextBlockHighlight"/>.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
