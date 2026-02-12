using Avalonia.Android;
using Plugin.Scanner.Avalonia.Android;

namespace Plugin.Scanner.Avalonia.Hosting;

/// <summary>
/// Provides initialization methods for the scanner plugin on Android.
/// </summary>
/// <example>
/// <code>
/// protected override void OnCreate(Bundle savedInstanceState)
/// {
///     base.OnCreate(savedInstanceState);
///     Scanner.Init(this);
/// }
/// </code>
/// </example>
public static class Scanner
{
    /// <summary>
    /// Initializes the scanner plugin with the specified Avalonia activity.
    /// </summary>
    /// <param name="avaloniaActivity">The Avalonia activity to register for lifecycle callbacks.</param>
    /// <remarks>
    /// This method must be called during the activity initialization to ensure proper scanner functionality.
    /// </remarks>
    public static void Init(AvaloniaActivity avaloniaActivity)
    {
        avaloniaActivity.Application?.RegisterActivityLifecycleCallbacks(Platform.ActivityLifecycleContextListener);
    }
}
