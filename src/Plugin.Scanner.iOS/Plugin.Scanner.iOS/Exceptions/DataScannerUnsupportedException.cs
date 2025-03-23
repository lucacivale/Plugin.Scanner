#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Exceptions;
#pragma warning restore SA1300

/// <inheritdoc />
public class DataScannerUnsupportedException : Exception
{
    /// <inheritdoc />
    public DataScannerUnsupportedException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
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