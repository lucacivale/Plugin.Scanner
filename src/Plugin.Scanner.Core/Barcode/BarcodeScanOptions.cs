#pragma warning disable IDE0005
using System.Collections.Generic;
#pragma warning restore IDE0005

namespace Plugin.Scanner.Core.Barcode;

/// <inheritdoc />
public sealed class BarcodeScanOptions : IBarcodeScanOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScanOptions"/> class.
    /// </summary>
    /// <param name="formats">Formats to be recognized.</param>
    public BarcodeScanOptions(IEnumerable<string> formats)
    {
        Formats = formats;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScanOptions"/> class.
    /// </summary>
    /// <param name="format">Format to be recognized.</param>
    public BarcodeScanOptions(string format)
    {
        Formats = [format];
    }

    /// <inheritdoc />
    public IEnumerable<string> Formats { get; set; }
}