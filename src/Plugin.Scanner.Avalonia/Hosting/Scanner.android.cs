using Avalonia.Android;
using Plugin.Scanner.Avalonia.Android;

namespace Plugin.Scanner.Avalonia.Hosting;

public static class Scanner
{
    public static void Init(AvaloniaActivity avaloniaActivity)
    {
        avaloniaActivity.Application?.RegisterActivityLifecycleCallbacks(Platform.ActivityLifecycleContextListener);
    }
}
