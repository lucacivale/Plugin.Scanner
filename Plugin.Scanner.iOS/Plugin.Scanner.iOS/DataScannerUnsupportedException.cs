#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public class DataScannerUnsupportedException : Exception
{
    public DataScannerUnsupportedException(string message)
        : base(message)
    {
    }

    public DataScannerUnsupportedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DataScannerUnsupportedException()
    {
    }
}