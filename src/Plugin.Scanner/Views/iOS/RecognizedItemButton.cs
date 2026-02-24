namespace Plugin.Scanner.Views.iOS;

/// <summary>
/// Represents a custom UIButton for displaying and interacting with a recognized barcode item.
/// </summary>
internal sealed class RecognizedItemButton : UIButton
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecognizedItemButton"/> class.
    /// Represents a custom UIButton for displaying and interacting with a recognized barcode item.
    /// </summary>
    /// <param name="barcode">RecognizedItem instance representing the barcode associated with this button.</param>
    public RecognizedItemButton()
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
    public Core.Models.RecognizedItem? Barcode
    {
        get;
        set
        {
            field = value;

            UIButtonConfiguration? config = (UIButtonConfiguration?)Configuration?.Copy();
            config?.Title = value?.Text;

            Configuration = config;
        }
    }
}