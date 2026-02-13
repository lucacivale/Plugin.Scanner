namespace Plugin.Scanner.Core.Exceptions;

/// <inheritdoc />
public sealed class BarcodeScanException : Exception
{
    /// <inheritdoc />
    public BarcodeScanException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public BarcodeScanException()
    {
    }

    /// <inheritdoc />
    public BarcodeScanException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}