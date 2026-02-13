namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when the data scanner is not supported on the current device.
/// </summary>
public sealed class DataScannerUnsupportedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerUnsupportedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerUnsupportedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerUnsupportedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerUnsupportedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerUnsupportedException"/> class.
    /// </summary>
    public DataScannerUnsupportedException()
    {
    }
}