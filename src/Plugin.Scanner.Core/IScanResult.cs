namespace Plugin.Scanner.Core;

/// <summary>
/// Represents the result of a scan operation.
/// </summary>
public interface IScanResult
{
    /// <summary>
    /// Gets the scanned value as a string.
    /// </summary>
    string Value { get; }
}