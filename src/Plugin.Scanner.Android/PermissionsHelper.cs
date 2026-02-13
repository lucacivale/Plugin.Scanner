using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace Plugin.Scanner.Android;

/// <summary>
/// Provides helper methods for checking and requesting camera and flashlight permissions.
/// </summary>
internal static class PermissionsHelper
{
    private const int RequestCodePermissions = 10;

    private static readonly string[] _requiredPermissions = [Manifest.Permission.Camera, Manifest.Permission.Flashlight];

    /// <summary>
    /// Requests the required camera and flashlight permissions from the user.
    /// </summary>
    /// <param name="activity">The activity to request permissions from.</param>
    public static void RequestPermissions(Activity activity)
    {
        ActivityCompat.RequestPermissions(activity, _requiredPermissions, RequestCodePermissions);
    }

    /// <summary>
    /// Checks if all required permissions are granted.
    /// </summary>
    /// <param name="context">The context to check permissions against.</param>
    /// <returns><c>true</c> if all required permissions are granted; otherwise, <c>false</c>.</returns>
    public static bool PermissionsGranted(Context context)
    {
        return _requiredPermissions.All(permission => ContextCompat.CheckSelfPermission(context, permission) == Permission.Granted);
    }

    /// <summary>
    /// Checks if permissions are granted and requests them if not.
    /// </summary>
    /// <param name="activity">The activity to check and request permissions from.</param>
    public static void CheckPermissions(Activity activity)
    {
        if (PermissionsGranted(activity) == false)
        {
            RequestPermissions(activity);
        }
    }
}
