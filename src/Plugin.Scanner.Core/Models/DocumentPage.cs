namespace Plugin.Scanner.Core.Models;

internal sealed class DocumentPage : IDocumentPage
{
    public DocumentPage(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}
