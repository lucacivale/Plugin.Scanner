using AndroidX.AppCompat.App;

namespace Plugin.Scanner.Android;

/// <summary>
/// Provides access to the current Android activity for barcode scanning operations.
/// </summary>
public sealed class CurrentActivity : ICurrentActivity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentActivity"/> class.
    /// </summary>
    /// <param name="getActivity">A function that returns the current activity.</param>
    public CurrentActivity(Func<AppCompatActivity> getActivity)
    {
        GetActivity = getActivity;
    }

    /// <summary>
    /// Gets a function that returns the current <see cref="AppCompatActivity"/>.
    /// </summary>
    public Func<AppCompatActivity> GetActivity { get; }
}