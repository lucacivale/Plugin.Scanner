using Android.Content.PM;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Context"/>.
/// </summary>
internal static class ContextExtensions
{
    /// <summary>
    /// Determines whether the device has a camera feature available.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <returns><c>true</c> if the device has a camera; otherwise, <c>false</c>.</returns>
    public static bool HasCamera(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCamera) == true;
    }
}
