using AndroidX.AppCompat.App;

namespace Plugin.Scanner.Android;

public sealed class CurrentActivity : ICurrentActivity
{
    public CurrentActivity(Func<AppCompatActivity> getActivity)
    {
        GetActivity = getActivity;
    }

    public Func<AppCompatActivity> GetActivity { get; }
}