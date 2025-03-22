#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Exceptions;
#pragma warning restore SA1300

/// <inheritdoc />
public sealed class DataScannerCapturePhotoException : Exception
{
    /// <inheritdoc />
    public DataScannerCapturePhotoException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
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