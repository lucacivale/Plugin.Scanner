using Plugin.Scanner.Android;
using AActivity = Android.App.Activity;

namespace Plugin.Scanner.Uno.Android;

/// <summary>
/// Provides access to the current Android activity instance.
/// </summary>
/// <remarks>
/// This implementation retrieves the current activity from <see cref="ContextHelper.Current"/>
/// and ensures it is valid before returning.
/// </remarks>
internal sealed class CurrentActivity : ICurrentActivity
{
    /// <summary>
    /// Gets the current Android activity instance.
    /// </summary>
    /// <value>
    /// The current <see cref="AActivity"/> instance.
    /// </value>
    /// <exception cref="NotSupportedException">
    /// Thrown when the current context is not an activity or is null.
    /// </exception>
    public AActivity Activity
    {
        get
        {
            if (ContextHelper.Current is not AActivity)
            {
                throw new NotSupportedException("Current activity can't be null here.");
            }

            return (AActivity)ContextHelper.Current;
        }
    }
}
