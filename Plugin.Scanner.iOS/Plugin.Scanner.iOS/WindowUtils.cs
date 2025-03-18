#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

internal static class WindowUtils
{
    internal static UIViewController? GetTopViewController()
    {
        NSSet<UIScene> scenes = UIApplication.SharedApplication.ConnectedScenes;
        UIWindowScene? windowScene = scenes.ToArray().OfType<UIWindowScene>().FirstOrDefault(scene =>
            scene.Session.Role == UIWindowSceneSessionRole.Application);
        UIWindow? window = windowScene?.Windows.FirstOrDefault();
        UIViewController? topViewController = window?.RootViewController;

        if (topViewController is not null
            && topViewController.PresentedViewController is not null)
        {
            topViewController = topViewController.PresentedViewController;

            while (topViewController?.PresentedViewController is not null)
            {
                topViewController = topViewController.PresentedViewController;
            }
        }

        return topViewController;
    }
}