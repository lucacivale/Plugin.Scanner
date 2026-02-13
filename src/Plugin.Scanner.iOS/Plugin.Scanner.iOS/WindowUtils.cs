namespace Plugin.Scanner.iOS;

/// <summary>
/// Provides utility methods for working with UIKit windows and view controllers.
/// </summary>
internal static class WindowUtils
{
    /// <summary>
    /// Gets the topmost visible view controller in the application's view hierarchy.
    /// </summary>
    /// <returns>
    /// The topmost <see cref="UIViewController"/> in the presentation chain, or <c>null</c> if no view controller is found.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method traverses the view hierarchy to find the currently visible view controller by:
    /// <list type="number">
    /// <item><description>Finding the active application window scene</description></item>
    /// <item><description>Getting the root view controller from the window</description></item>
    /// <item><description>Traversing any presented view controllers to find the topmost one</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This is useful for presenting modal view controllers when the current context is unknown.
    /// </para>
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