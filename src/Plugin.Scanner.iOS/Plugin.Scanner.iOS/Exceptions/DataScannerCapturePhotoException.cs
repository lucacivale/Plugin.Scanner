namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when a photo capture operation fails.
/// </summary>
public sealed class DataScannerCapturePhotoException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCapturePhotoException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerCapturePhotoException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCapturePhotoException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerCapturePhotoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCapturePhotoException"/> class.
    /// </summary>
    public DataScannerCapturePhotoException()
    {
    }
}