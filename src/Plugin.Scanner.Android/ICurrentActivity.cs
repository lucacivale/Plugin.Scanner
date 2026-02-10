using AndroidX.AppCompat.App;

namespace Plugin.Scanner.Android;

public interface ICurrentActivity
{
    Func<AppCompatActivity> GetActivity { get; }
}