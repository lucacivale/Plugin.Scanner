namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when the data scanner's view is unexpectedly null.
/// </summary>
public sealed class DataScannerViewNullReferenceException : NullReferenceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewNullReferenceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerViewNullReferenceException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewNullReferenceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerViewNullReferenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewNullReferenceException"/> class.
    /// </summary>
    public DataScannerViewNullReferenceException()
    {
    }
}