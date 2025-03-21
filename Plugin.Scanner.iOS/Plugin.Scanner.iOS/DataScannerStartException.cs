#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

public class DataScannerStartException : Exception
{
    public DataScannerStartException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DataScannerStartException()
    {
    }

    public DataScannerStartException(string message)
        : base(message)
    {
    }
}