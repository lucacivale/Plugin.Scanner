using Android.Content.PM;

namespace Plugin.Scanner.Android.Extensions;

internal static class ContextExtensions
{
    public static bool HasCamera(this Context context)
    {
        return context.PackageManager?.HasSystemFeature(PackageManager.FeatureCamera) == true;
    }
}
