namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when an event sender is not of the expected type.
/// </summary>
public sealed class DataScannerEventSenderInvalidTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerEventSenderInvalidTypeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerEventSenderInvalidTypeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerEventSenderInvalidTypeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerEventSenderInvalidTypeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerEventSenderInvalidTypeException"/> class.
    /// </summary>
    public DataScannerEventSenderInvalidTypeException()
    {
    }
}