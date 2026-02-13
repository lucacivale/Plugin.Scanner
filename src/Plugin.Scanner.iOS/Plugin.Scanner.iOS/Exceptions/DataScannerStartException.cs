namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when the data scanner fails to start.
/// </summary>
public sealed class DataScannerStartException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerStartException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerStartException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerStartException"/> class.
    /// </summary>
    public DataScannerStartException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerStartException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerStartException(string message)
        : base(message)
    {
    }
}