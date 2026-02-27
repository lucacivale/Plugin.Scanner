namespace Plugin.Scanner.Core.Models;

internal sealed class ScanResult : IScanResult
{
    public ScanResult(string value)
    {
        Value = value;
    }

    public string Value { get; }
}