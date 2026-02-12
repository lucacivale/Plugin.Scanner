using Android.App;
using Plugin.Scanner.Android;

namespace Plugin.Scanner.Maui.Android;

/// <summary>
/// Provides access to the current Android activity.
/// </summary>
internal sealed class CurrentActivity : ICurrentActivity
{
    /// <summary>
    /// Gets the current Android activity.
    /// </summary>
    /// <value>
    /// The current <see cref="Activity"/> instance.
    /// </value>
    /// <exception cref="NotSupportedException">
    /// Thrown when the current activity is null.
    /// </exception>
    public Activity Activity => Platform.CurrentActivity ?? throw new NotSupportedException("Current activity can't be null here.");
}
