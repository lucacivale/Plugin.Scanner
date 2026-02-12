using Plugin.Scanner.Android;

namespace Plugin.Scanner.Avalonia.Android;

/// <summary>
/// Provides access to the current Android activity for the scanner plugin.
/// </summary>
internal sealed class CurrentActivity : ICurrentActivity
{
    /// <summary>
    /// Gets the current active activity.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when no current activity is available.</exception>
    public Activity Activity => Platform.CurrentActivity ?? throw new NotSupportedException("Current activity can't be null here.");
}
