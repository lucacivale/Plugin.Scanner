using Avalonia;
using Android.Content.PM;
using Avalonia.Android;

namespace Plugin.Scanner.Avalonia.Tests.Android;

[Activity(
    Label = "Plugin.Scanner.Avalonia.Tests.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
     
        Hosting.Scanner.Init(this);
    }
}
