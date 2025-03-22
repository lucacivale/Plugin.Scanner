#pragma warning disable SA1300
namespace Plugin.Scanner.iOS.Barcode;
#pragma warning restore SA1300

/// <inheritdoc />
public class DataScannerBarcodeScanException : Exception
{
    /// <inheritdoc />
    public DataScannerBarcodeScanException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public DataScannerBarcodeScanException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerBarcodeScanException"/> class.
    /// </summary>
    public DataScannerBarcodeScanException()
    {
    }
}