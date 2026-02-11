namespace Plugin.Scanner.Avalonia.Android;

internal sealed class ActivityLifecycleContextListener : Application, Application.IActivityLifecycleCallbacks
{
    private readonly WeakReference<Activity?> _currentActivity = new(null);

    public Activity? Activity
    {
        get => _currentActivity.TryGetTarget(out Activity? a) ? a : null;
        set => _currentActivity.SetTarget(value);
    }

    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
    {
        Activity = activity;
    }

    public void OnActivityDestroyed(Activity activity)
    {
    }

    public void OnActivityPaused(Activity activity)
    {
    }

    public void OnActivityResumed(Activity activity)
    {
        Activity = activity;
    }

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
    {
    }

    public void OnActivityStarted(Activity activity)
    {
        Activity = activity;
    }

    public void OnActivityStopped(Activity activity)
    {
    }
}
