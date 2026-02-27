namespace Plugin.Scanner.Core.Exceptions;

/// <inheritdoc />
public sealed class ScanException : Exception
{
    /// <inheritdoc />
    public ScanException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public ScanException()
    {
    }

    /// <inheritdoc />
    public ScanException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}