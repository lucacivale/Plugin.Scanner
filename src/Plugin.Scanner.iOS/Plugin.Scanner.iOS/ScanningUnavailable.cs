#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// The possible reasons the data scanner is unavailable.
/// </summary>
public enum ScanningUnavailable
{
    /// <summary>
    /// The data scanner isn’t supported on this device.
    /// </summary>
    Unsupported,

    /// <summary>
    /// The data scanner isn’t available due to user restrictions on the use of the camera.
    /// </summary>
    CameraRestricted,
}

/// <summary>
/// <see cref="ScanningUnavailable"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1649: File name should match first type name", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1402: A C# code file contains more than one unique type.", Justification = "Is ok here.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "SA1204: A static element is positioned beneath an instance element of the same type.", Justification = "Is ok here.")]
public static class ScanningUnavailableExtensions
{
    /// <summary>
    /// To <see cref="Plugin.Scanner.iOS.Binding.ScanningUnavailable"/>.
    /// </summary>
    /// <param name="scanningUnavailable"><see cref="ScanningUnavailable"/>.</param>
    /// <returns>Returns <see cref="Plugin.Scanner.iOS.Binding.ScanningUnavailable"/>.</returns>
    public static Plugin.Scanner.iOS.Binding.ScanningUnavailable ToVnScanningUnavailable(this ScanningUnavailable scanningUnavailable)
    {
        return scanningUnavailable switch
        {
            ScanningUnavailable.Unsupported => Plugin.Scanner.iOS.Binding.ScanningUnavailable.Unsupported,
            ScanningUnavailable.CameraRestricted => Plugin.Scanner.iOS.Binding.ScanningUnavailable.CameraRestricted,
            _ => Plugin.Scanner.iOS.Binding.ScanningUnavailable.Unsupported,
        };
    }

    /// <summary>
    /// To <see cref="ScanningUnavailable"/>.
    /// </summary>
    /// <param name="scanningUnavailable"><see cref="Plugin.Scanner.iOS.Binding.ScanningUnavailable"/>.</param>
    /// <returns>Returns <see cref="ScanningUnavailable"/>.</returns>
    public static ScanningUnavailable ToVnScanningUnavailable(this Plugin.Scanner.iOS.Binding.ScanningUnavailable scanningUnavailable)
    {
        return scanningUnavailable switch
        {
            Plugin.Scanner.iOS.Binding.ScanningUnavailable.Unsupported => ScanningUnavailable.Unsupported,
            Plugin.Scanner.iOS.Binding.ScanningUnavailable.CameraRestricted => ScanningUnavailable.CameraRestricted,
            _ => ScanningUnavailable.Unsupported,
        };
    }
}