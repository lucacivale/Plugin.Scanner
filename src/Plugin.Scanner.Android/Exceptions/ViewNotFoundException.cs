namespace Plugin.Scanner.Android.Exceptions;

/// <summary>
/// Exception thrown when a required view cannot be found in the scanner dialog layout.
/// </summary>
public sealed class ViewNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewNotFoundException"/> class with the name of the missing view.
    /// </summary>
    /// <param name="viewName">The name of the view that was not found.</param>
    public ViewNotFoundException(string viewName)
        : base($"{viewName} not found in layout.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewNotFoundException"/> class with the name of the missing view and inner exception.
    /// </summary>
    /// <param name="viewName">The name of the view that was not found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ViewNotFoundException(string viewName, Exception innerException)
        : base($"{viewName} not found in layout.", innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewNotFoundException"/> class.
    /// </summary>
    public ViewNotFoundException()
    {
    }
}