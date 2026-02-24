using System.ComponentModel;
using System.Drawing;
using Android.Util;
using Plugin.Scanner.Core.Models;
using AColor = Android.Graphics.Color;

namespace Plugin.Scanner.Views.Android;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RecognizedItemButton : Button
{
    public RecognizedItemButton(Context context)
        : base(context)
    {
        Init();
    }

    public RecognizedItemButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    public RecognizedItemButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    public EventHandler<RecognizedItem>? Clicked { get; set; }

    public RecognizedItem? RecognizedItem
    {
        get => field;
        set
        {
            field = value;
            Text = value?.Text ?? string.Empty;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= BarcodeItemButton_Click;
        }

        base.Dispose(disposing);
    }

    private void Init()
    {
        TextAlignment = TextAlignment.Center;
        Ellipsize = TextUtils.TruncateAt.Middle;
        SetSingleLine(true);

        SetBackgroundColor(AColor.Yellow);
        SetTextColor(AColor.Black);

        SetPadding(36, 20, 36, 20);

        GradientDrawable background = new();

        background.SetShape(ShapeType.Rectangle);
        background.SetCornerRadius(1000f);
        background.SetColor(AColor.Yellow);

        Background = background;

        Click += BarcodeItemButton_Click;
    }

    private void BarcodeItemButton_Click(object? sender, EventArgs e)
    {
        if (RecognizedItem is not null)
        {
            Clicked?.Invoke(this, RecognizedItem);
        }
    }
}
