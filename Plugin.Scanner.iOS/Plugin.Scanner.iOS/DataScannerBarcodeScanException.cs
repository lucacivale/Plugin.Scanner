#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public class DataScannerBarcodeScanException : Exception
{
    public DataScannerBarcodeScanException(string message)
        : base(message)
    {
    }

    public DataScannerBarcodeScanException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DataScannerBarcodeScanException()
    {
    }
}