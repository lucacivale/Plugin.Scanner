namespace Plugin.Scanner.iOS.Views;

/// <summary>
/// A visual effect view that provides a blurred overlay bar for the data scanner interface.
/// </summary>
/// <remarks>
/// This overlay uses the system ultra-thin material dark blur effect to create a semi-transparent bar
/// that can be positioned at the top or bottom of the scanner view.
/// </remarks>
internal sealed class DataScannerBarOverlay : UIVisualEffectView
{
    /// <summary>
    /// The fixed height of the overlay bar in points.
    /// </summary>
    public const float Height = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerBarOverlay"/> class.
    /// </summary>
    /// <remarks>
    /// The overlay is configured with a dark ultra-thin material blur effect, and Auto Layout is enabled.
    /// </remarks>
    public DataScannerBarOverlay()
        : base(UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemUltraThinMaterialDark))
    {
        TranslatesAutoresizingMaskIntoConstraints = false;
    }
}