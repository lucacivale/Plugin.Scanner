namespace Plugin.Scanner.Avalonia.Android;

/// <summary>
/// Tracks the current active Android activity throughout the application lifecycle.
/// </summary>
internal sealed class ActivityLifecycleContextListener : Application, Application.IActivityLifecycleCallbacks
{
    private readonly WeakReference<Activity?> _currentActivity = new(null);

    /// <summary>
    /// Gets or sets the current active activity.
    /// </summary>
    public Activity? Activity
    {
        get => _currentActivity.TryGetTarget(out Activity? a) ? a : null;
        set => _currentActivity.SetTarget(value);
    }

    /// <inheritdoc/>
    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
    {
        Activity = activity;
    }

    /// <inheritdoc/>
    public void OnActivityDestroyed(Activity activity)
    {
    }

    /// <inheritdoc/>
    public void OnActivityPaused(Activity activity)
    {
    }

    /// <inheritdoc/>
    public void OnActivityResumed(Activity activity)
    {
        Activity = activity;
    }

    /// <inheritdoc/>
    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
    {
    }

    /// <inheritdoc/>
    public void OnActivityStarted(Activity activity)
    {
        Activity = activity;
    }

    /// <inheritdoc/>
    public void OnActivityStopped(Activity activity)
    {
    }
}
