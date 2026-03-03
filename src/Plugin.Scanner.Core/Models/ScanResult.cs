namespace Plugin.Scanner.Core.Models;

/// <summary>
/// Represents the result of a scan operation.
/// </summary>
internal sealed class ScanResult : IScanResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScanResult"/> class.
    /// </summary>
    /// <param name="value">The scanned value.</param>
    public ScanResult(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the scanned value.
    /// </summary>
    public string Value { get; }
}