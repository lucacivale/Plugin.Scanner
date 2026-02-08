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
    public BarcodeItemButton(RecognizedItem barcode)
    {
        Barcode = barcode;

        TranslatesAutoresizingMaskIntoConstraints = false;

        UIButtonConfiguration config = UIButtonConfiguration.FilledButtonConfiguration;
        config.Title = barcode.Value;
        config.BaseBackgroundColor = UIColor.Yellow;
        config.BaseForegroundColor = UIColor.Black;
        config.CornerStyle = UIButtonConfigurationCornerStyle.Capsule;
        config.ContentInsets = new NSDirectionalEdgeInsets(10, 18, 10, 18);
        config.TitleLineBreakMode = UILineBreakMode.MiddleTruncation;
        config.TitleAlignment = UIButtonConfigurationTitleAlignment.Center;

        Configuration = config;
    }

    /// <summary>
    /// Gets the RecognizedItem instance representing the barcode associated with this button.
    /// </summary>
    public RecognizedItem Barcode { get; }
}