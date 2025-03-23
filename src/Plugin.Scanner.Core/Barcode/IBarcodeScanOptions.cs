#pragma warning disable IDE0005
using System.Collections.Generic;
#pragma warning restore IDE0005

namespace Plugin.Scanner.Core.Barcode;

/// <summary>
/// Options to configure barcode scan.
/// </summary>
public interface IBarcodeScanOptions
{
    /// <summary>
    /// Gets formats to be recognized.
    /// </summary>
    public IEnumerable<string> Formats { get; }
}