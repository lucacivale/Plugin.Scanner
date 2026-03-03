namespace Plugin.Scanner.Android;

/// <summary>
/// Provides access to the current Android activity.
/// </summary>
public interface ICurrentActivity
{
    /// <summary>
    /// Gets the current Android activity.
    /// </summary>
    Activity Activity { get; }
}