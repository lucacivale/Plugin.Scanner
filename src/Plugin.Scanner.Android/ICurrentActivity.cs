namespace Plugin.Scanner.Android;

/// <summary>
/// Defines a contract for accessing the current Android activity.
/// </summary>
public interface ICurrentActivity
{
    Activity Activity { get; }
}