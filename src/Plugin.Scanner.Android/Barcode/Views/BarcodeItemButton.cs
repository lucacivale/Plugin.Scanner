using Android.Util;

namespace Plugin.Scanner.Android.Barcode.Views;

public sealed class BarcodeItemButton : Button
{
    public BarcodeItemButton(Context context, string barcodeValue)
        : base(context)
    {
        Init();
    }

    public BarcodeItemButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    public BarcodeItemButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    public EventHandler<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>? Clicked { get; set; }

    public Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode? Barcode
    {
        get => field;
        set
        {
            field = value;
            Text = value?.DisplayValue ?? string.Empty;
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

        SetBackgroundColor(Color.Yellow);
        SetTextColor(Color.Black);

        SetPadding(36, 20, 36, 20);

        GradientDrawable background = new();

        background.SetShape(ShapeType.Rectangle);
        background.SetCornerRadius(1000f);
        background.SetColor(Color.Yellow);

        Background = background;

        Click += BarcodeItemButton_Click;
    }

    private void BarcodeItemButton_Click(object? sender, EventArgs e)
    {
        Clicked?.Invoke(this, Barcode);
    }
}
