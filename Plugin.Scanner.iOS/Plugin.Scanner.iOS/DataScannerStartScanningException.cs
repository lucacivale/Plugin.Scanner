#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public class DataScannerStartScanningException : Exception
{
    public DataScannerStartScanningException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DataScannerStartScanningException()
    {
    }

    public DataScannerStartScanningException(string message)
        : base(message)
    {
    }
}