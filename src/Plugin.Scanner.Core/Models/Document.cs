namespace Plugin.Scanner.Core.Models;

internal sealed class Document : IDocument
{
    public Document(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}