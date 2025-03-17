#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <inheritdoc />
public sealed class CapturePhotoException : Exception
{
    public CapturePhotoException(string message)
        : base(message)
    {
    }

    public CapturePhotoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public CapturePhotoException()
    {
    }
}