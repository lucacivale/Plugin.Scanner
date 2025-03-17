#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// The possible quality levels that the scanner uses to find data. The quality levels mostly impact the camera resolution.
/// </summary>
public enum QualityLevel
{
    // A quality level that’s between fast and accurate.
    Balanced,

    // A quality level that prioritizes recognition speed over accuracy.
    // This quality level may fail to recognize smaller text and barcodes.
    Fast,

    // Use this quality level if you want to recognize smaller text and
    // barcodes.
    Accurate,
}

/// <summary>
/// <see cref="QualityLevel"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1649: File name should match first type name", Justification = "Is ok here.")]
public static class QualityLevelExtensions
{
    /// <summary>
    /// Create native QualityLevel enum.
    /// </summary>
    /// <param name="qualityLevel"><see cref="QualityLevel"/>.</param>
    /// <returns>Native quality level.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Throws if no matching quality level is found.</exception>
    public static Plugin.Scanner.iOS.Binding.QualityLevel ToVnQualityLevel(this QualityLevel qualityLevel)
    {
        return qualityLevel switch
        {
            QualityLevel.Balanced => Plugin.Scanner.iOS.Binding.QualityLevel.Balanced,
            QualityLevel.Accurate => Plugin.Scanner.iOS.Binding.QualityLevel.Accurate,
            QualityLevel.Fast => Plugin.Scanner.iOS.Binding.QualityLevel.Fast,
            _ => throw new ArgumentOutOfRangeException(nameof(qualityLevel), qualityLevel, null),
        };
    }
}