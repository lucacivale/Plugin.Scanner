#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public class DataScannerUnavailableException : Exception
{
    public DataScannerUnavailableException(string message)
        : base(message)
    {
    }

    public DataScannerUnavailableException()
    {
    }

    public DataScannerUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}