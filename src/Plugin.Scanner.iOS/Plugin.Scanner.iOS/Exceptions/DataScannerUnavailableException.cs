#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Exceptions;
#pragma warning restore SA1300

/// <inheritdoc />
public class DataScannerUnavailableException : Exception
{
    /// <inheritdoc />
    public DataScannerUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerUnavailableException"/> class.
    /// </summary>
    public DataScannerUnavailableException()
    {
    }

    /// <inheritdoc />
    public DataScannerUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}