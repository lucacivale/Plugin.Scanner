#pragma warning disable IDE0005
using System;
#pragma warning restore IDE0005

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