namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when the data scanner view controller cannot be found in the view hierarchy.
/// </summary>
public sealed class DataScannerViewControllerNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewControllerNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerViewControllerNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewControllerNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerViewControllerNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewControllerNotFoundException"/> class.
    /// </summary>
    public DataScannerViewControllerNotFoundException()
    {
    }
}