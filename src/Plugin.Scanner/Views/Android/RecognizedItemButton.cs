using System.ComponentModel;
using Android.Util;
using Plugin.Scanner.Core.Models;
using AColor = Android.Graphics.Color;

namespace Plugin.Scanner.Views.Android;

/// <summary>
/// A button that displays and handles clicks on recognized items from the scanner.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RecognizedItemButton : Button
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItemButton"/> class.
    /// </summary>
    /// <param name="context">The Android context.</param>
    public RecognizedItemButton(Context context)
        : base(context)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItemButton"/> class with the specified attribute set.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    public RecognizedItemButton(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItemButton"/> class with the specified attribute set and style.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The attribute set from XML layout.</param>
    /// <param name="defStyleAttr">The default style attribute.</param>
    public RecognizedItemButton(Context context, IAttributeSet attrs, int defStyleAttr)
        : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    /// <summary>
    /// Gets or sets the button is clicked with a recognized item.
    /// </summary>
    public EventHandler<RecognizedItem>? Clicked { get; set; }

    /// <summary>
    /// Gets or sets the recognized item associated with this button.
    /// </summary>
    public RecognizedItem? RecognizedItem
    {
        get => field;
        set
        {
            field = value;
            Text = value?.Text ?? string.Empty;
        }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="RecognizedItemButton"/>.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Click -= BarcodeItemButton_Click;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Initializes the button's appearance and subscribes to click events.
    /// </summary>
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

    /// <summary>
    /// Handles the button click and raises the Clicked event with the recognized item.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void BarcodeItemButton_Click(object? sender, EventArgs e)
    {
        if (RecognizedItem is not null)
        {
            Clicked?.Invoke(this, RecognizedItem);
        }
    }
}
