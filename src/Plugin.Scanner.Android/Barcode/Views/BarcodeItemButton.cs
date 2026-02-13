using System.ComponentModel;
using Android.Util;

namespace Plugin.Scanner.Android.Barcode.Views;

/// <summary>
/// Represents a custom button that displays a scanned barcode value with a pill-shaped design.
/// </summary>
/// <remarks>
/// <para>
/// This button is displayed in the scanning interface to show the currently detected barcode
/// and allow the user to confirm the selection by tapping it.
/// </para>
/// <para>
/// The button features:
/// <list type="bullet">
/// <item><description>Yellow background with black text</description></item>
/// <item><description>Rounded pill-shaped appearance</description></item>
/// <item><description>Center-aligned text with middle ellipsis for long values</description></item>
/// <item><description>Single-line display</description></item>
/// </list>
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class BarcodeItemButton : Button
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeItemButton"/> class with the specified context and barcode value.
    /// </summary>
    /// <param name="context">The Android context.</param>
    public BarcodeItemButton(Context context)
        : base(context)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeItemButton"/> class with the specified context and attribute set.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    public BarcodeItemButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeItemButton"/> class with the specified context, attribute set, and style.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    /// <param name="defStyleAttr">The default style attribute.</param>
    public BarcodeItemButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    /// <summary>
    /// Gets or sets Occurs when the button is clicked, providing the associated barcode object.
    /// </summary>
    /// <value>
    /// An event handler that receives the <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/>
    /// object associated with this button when clicked.
    /// </value>
    public EventHandler<Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode>? Clicked { get; set; }

    /// <summary>
    /// Gets or sets the barcode object associated with this button.
    /// </summary>
    /// <value>
    /// The <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode"/> object, or <c>null</c> if no barcode is set.
    /// </value>
    /// <remarks>
    /// When a barcode is set, the button's text is automatically updated to display the barcode's
    /// <see cref="Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode.DisplayValue"/>.
    /// If the barcode is <c>null</c> or has no display value, the button text is cleared.
    /// </remarks>
    public Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode? Barcode
    {
        get => field;
        set
        {
            field = value;
            Text = value?.DisplayValue ?? string.Empty;
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="BarcodeItemButton"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= BarcodeItemButton_Click;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Initializes the button's visual appearance and behavior.
    /// </summary>
    /// <remarks>
    /// This method configures:
    /// <list type="bullet">
    /// <item><description>Center text alignment with middle ellipsis truncation</description></item>
    /// <item><description>Single-line text display</description></item>
    /// <item><description>Yellow background with black text</description></item>
    /// <item><description>Rounded pill-shaped background (1000px corner radius)</description></item>
    /// <item><description>Padding of 36px horizontal and 20px vertical</description></item>
    /// <item><description>Click event handler</description></item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// Handles the button click event and raises the <see cref="Clicked"/> event with the associated barcode.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void BarcodeItemButton_Click(object? sender, EventArgs e)
    {
        if (Barcode is not null)
        {
            Clicked?.Invoke(this, Barcode);
        }
    }
}
