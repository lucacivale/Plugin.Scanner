using Android.Content.PM;
using Android.Util;
using System.Runtime.CompilerServices;

namespace Plugin.Scanner.Android.Extensions;

/// <summary>
/// Provides extension methods for Android <see cref="Context"/> for density conversion and feature detection.
/// </summary>
internal static class ContextExtensions
{
    private static float _displayDensity = float.MinValue;

    /// <summary>
    /// Converts density-independent pixels (dp) to screen pixels.
    /// </summary>
    /// <param name="self">The context to get display metrics from.</param>
    /// <param name="dp">The value in density-independent pixels.</param>
    /// <returns>The equivalent value in screen pixels.</returns>
    public static float ToPixels(this Context self, double dp)
    {
        EnsureMetrics(self);

        return ToPixelsUsingMetrics(dp);
    }

    /// <summary>
    /// Converts screen pixels to density-independent pixels (dp).
    /// </summary>
    /// <param name="self">The context to get display metrics from.</param>
    /// <param name="pixels">The value in screen pixels.</param>
    /// <returns>The equivalent value in density-independent pixels.</returns>
    public static double FromPixels(this Context self, double pixels)
    {
        EnsureMetrics(self);

        return FromPixelsUsingMetrics(pixels);
    }

    /// <summary>
    /// Checks if the device has a camera.
    /// </summary>
    /// <param name="context">The context to check features from.</param>
    /// <returns><c>true</c> if the device has a camera; otherwise, <c>false</c>.</returns>
    public static bool HasCamera(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCamera) == true;
    }

    /// <summary>
    /// Checks if the device has a camera flash.
    /// </summary>
    /// <param name="context">The context to check features from.</param>
    /// <returns><c>true</c> if the device has a flash; otherwise, <c>false</c>.</returns>
    public static bool HasFlash(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCameraFlash) == true;
    }

    /// <summary>
    /// Ensures display metrics are loaded and cached.
    /// </summary>
    /// <param name="context">The context to get display metrics from.</param>
    private static void EnsureMetrics(Context context)
    {
        if (Math.Abs(_displayDensity - float.MinValue) > 0)
        {
            return;
        }

        using DisplayMetrics? metrics = context.Resources?.DisplayMetrics;

        _displayDensity = metrics?.Density ?? 1;
    }

    /// <summary>
    /// Converts dp to pixels using cached display density.
    /// </summary>
    /// <param name="dp">The value in density-independent pixels.</param>
    /// <returns>The equivalent value in screen pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ToPixelsUsingMetrics(double dp)
    {
        return (float)Math.Ceiling((dp * _displayDensity) - 0.0000000001f);
    }

    /// <summary>
    /// Converts pixels to dp using cached display density.
    /// </summary>
    /// <param name="pixels">The value in screen pixels.</param>
    /// <returns>The equivalent value in density-independent pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double FromPixelsUsingMetrics(double pixels)
    {
        return pixels / _displayDensity;
    }
}
