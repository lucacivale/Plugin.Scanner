namespace Plugin.Scanner.Android;

/// <summary>
/// Defines a contract for accessing the current Android activity.
/// </summary>
public interface ICurrentActivity
{
    /// <summary>
    /// Gets the current Android activity instance.
    /// </summary>
    Activity Activity { get; }
}