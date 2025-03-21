#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <inheritdoc />
public sealed class DataScannerCapturePhotoException : Exception
{
    public DataScannerCapturePhotoException(string message)
        : base(message)
    {
    }

    public DataScannerCapturePhotoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DataScannerCapturePhotoException()
    {
    }
}