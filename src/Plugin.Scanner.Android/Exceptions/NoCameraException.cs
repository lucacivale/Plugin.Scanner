namespace Plugin.Scanner.Android.Exceptions;

/// <summary>
/// Exception thrown when the device has no camera available for barcode scanning.
/// </summary>
public sealed class NoCameraException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoCameraException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoCameraException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoCameraException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NoCameraException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoCameraException"/> class.
    /// </summary>
    public NoCameraException()
    {
    }
}