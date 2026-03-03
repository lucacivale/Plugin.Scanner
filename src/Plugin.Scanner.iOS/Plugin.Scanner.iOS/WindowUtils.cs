namespace Plugin.Scanner.iOS;

/// <summary>
/// Provides utility methods for working with iOS windows and view controllers.
/// </summary>
internal static class WindowUtils
{
    /// <summary>
    /// Gets the topmost view controller in the current window hierarchy.
    /// </summary>
    /// <returns>
    /// The topmost <see cref="UIViewController"/> if available; otherwise, <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method traverses the view controller hierarchy by checking for presented view controllers
    /// and returns the one at the top of the presentation stack.
    /// </remarks>
    public static UIViewController? GetTopViewController()
    {
        NSSet<UIScene> scenes = UIApplication.SharedApplication.ConnectedScenes;
        UIWindowScene? windowScene = scenes.ToArray().OfType<UIWindowScene>().FirstOrDefault(scene => scene.Session.Role == UIWindowSceneSessionRole.Application);
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