#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Exceptions;
#pragma warning restore SA1300

/// <inheritdoc />
public class DataScannerStartException : Exception
{
    /// <inheritdoc />
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

    /// <inheritdoc />
    public DataScannerStartException(string message)
        : base(message)
    {
    }
}