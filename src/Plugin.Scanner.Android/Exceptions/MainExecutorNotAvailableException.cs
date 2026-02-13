namespace Plugin.Scanner.Android.Exceptions;

/// <summary>
/// Exception thrown when the Android main executor is not available for camera operations.
/// </summary>
public sealed class MainExecutorNotAvailableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainExecutorNotAvailableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MainExecutorNotAvailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainExecutorNotAvailableException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MainExecutorNotAvailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainExecutorNotAvailableException"/> class.
    /// </summary>
    public MainExecutorNotAvailableException()
    {
    }
}