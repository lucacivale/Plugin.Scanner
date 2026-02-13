namespace Plugin.Scanner.iOS.Exceptions;

/// <summary>
/// The exception that is thrown when the camera configuration cannot be locked for modifications.
/// </summary>
public sealed class DataScannerCameraConfigurationLockException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCameraConfigurationLockException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataScannerCameraConfigurationLockException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCameraConfigurationLockException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataScannerCameraConfigurationLockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerCameraConfigurationLockException"/> class.
    /// </summary>
    public DataScannerCameraConfigurationLockException()
    {
    }
}