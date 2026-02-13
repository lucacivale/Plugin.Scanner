namespace Plugin.Scanner.Avalonia.Android;

/// <summary>
/// Provides platform-specific access to Android activity lifecycle information.
/// </summary>
internal static class Platform
{
    /// <summary>
    /// Gets the activity lifecycle listener that tracks the current activity.
    /// </summary>
    internal static ActivityLifecycleContextListener ActivityLifecycleContextListener { get; } = new();

    /// <summary>
    /// Gets the current active Android activity, if available.
    /// </summary>
    internal static Activity? CurrentActivity => ActivityLifecycleContextListener.Activity;
}
