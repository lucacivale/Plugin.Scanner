using AndroidX.AppCompat.App;

namespace Plugin.Scanner.Android;

/// <summary>
/// Defines a contract for accessing the current Android activity.
/// </summary>
public interface ICurrentActivity
{
    /// <summary>
    /// Gets a function that returns the current <see cref="AppCompatActivity"/>.
    /// </summary>
    Func<AppCompatActivity> GetActivity { get; }
}