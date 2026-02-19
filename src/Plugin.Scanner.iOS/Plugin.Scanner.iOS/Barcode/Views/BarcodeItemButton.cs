using Plugin.Scanner.iOS.Binding;

namespace Plugin.Scanner.iOS.Barcode.Views;

/// <summary>
/// Represents a custom UIButton for displaying and interacting with a recognized barcode item.
/// </summary>
internal sealed class BarcodeItemButton : UIButton
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeItemButton"/> class.
    /// Represents a custom UIButton for displaying and interacting with a recognized barcode item.
    /// </summary>
    /// <param name="barcode">RecognizedItem instance representing the barcode associated with this button.</param>
    public BarcodeItemButton()
    {
        Hidden = true;
        TranslatesAutoresizingMaskIntoConstraints = false;

        UIButtonConfiguration config = UIButtonConfiguration.FilledButtonConfiguration;
        config.BaseBackgroundColor = UIColor.Yellow;
        config.BaseForegroundColor = UIColor.Black;
        config.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        config.ContentInsets = new NSDirectionalEdgeInsets(10, 18, 10, 18);
        config.TitleLineBreakMode = UILineBreakMode.MiddleTruncation;
        config.TitleAlignment = UIButtonConfigurationTitleAlignment.Center;

        Configuration = config;
    }

    /// <summary>
    /// Gets or sets the RecognizedItem instance representing the barcode associated with this button.
    /// </summary>
    public RecognizedItem? Barcode
    {
        get;
        set
        {
            field = value;

            UIButtonConfiguration? config = (UIButtonConfiguration?)Configuration?.Copy();
            config?.Title = value?.Value;

            Configuration = config;
        }
    }
}