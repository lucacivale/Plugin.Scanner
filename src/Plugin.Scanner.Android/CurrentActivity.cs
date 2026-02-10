namespace Plugin.Scanner.Android;

public sealed class CurrentActivity : ICurrentActivity
{
    public CurrentActivity(Func<Activity?> getActivity)
    {
        GetActivity = getActivity;
    }

    public Func<Activity?> GetActivity { get; }
}