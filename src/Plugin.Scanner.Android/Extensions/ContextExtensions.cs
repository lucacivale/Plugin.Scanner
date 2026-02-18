using Android.Content.PM;
using Android.Util;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Context"/>.
/// </summary>
internal static class ContextExtensions
{
    private static float _displayDensity = float.MinValue;

    public static float ToPixels(this Context self, double dp)
    {
        EnsureMetrics(self);

        return ToPixelsUsingMetrics(dp);
    }

    /// <summary>
    /// Determines whether the device has a camera feature available.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <returns><c>true</c> if the device has a camera; otherwise, <c>false</c>.</returns>
    public static bool HasCamera(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCamera) == true;
    }

    public static bool HasFlash(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCameraFlash) == true;
    }

    private static void EnsureMetrics(Context context)
    {
        if (Math.Abs(_displayDensity - float.MinValue) > 0)
        {
            return;
        }

        using DisplayMetrics? metrics = context.Resources?.DisplayMetrics;

        _displayDensity = metrics?.Density ?? 1;
    }

    private static float ToPixelsUsingMetrics(double dp)
    {
        return (float)Math.Ceiling((dp * _displayDensity) - 0.0000000001f);
    }
}
