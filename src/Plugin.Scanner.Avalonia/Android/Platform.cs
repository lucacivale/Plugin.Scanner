namespace Plugin.Scanner.Avalonia.Android;

internal static class Platform
{
    internal static ActivityLifecycleContextListener ActivityLifecycleContextListener { get; } = new();

    internal static Activity? CurrentActivity => ActivityLifecycleContextListener.Activity;
}
