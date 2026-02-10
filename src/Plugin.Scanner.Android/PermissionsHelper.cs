using Android;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace Plugin.Scanner.Android;

internal static class PermissionsHelper
{
    private const int RequestCodePermissions = 10;

    private static readonly string[] _requiredPermissions = [Manifest.Permission.Camera, Manifest.Permission.Flashlight];

    public static void RequestPermissions(AppCompatActivity activity)
    {
        ActivityCompat.RequestPermissions(activity, _requiredPermissions, RequestCodePermissions);
    }

    public static bool PermissionsGranted(Context context)
    {
        return _requiredPermissions.All(permission => ContextCompat.CheckSelfPermission(context, permission) == Permission.Granted);
    }

    public static void CheckPermissions(AppCompatActivity activity)
    {
        if (PermissionsGranted(activity) == false)
        {
            RequestPermissions(activity);
        }
    }
}
