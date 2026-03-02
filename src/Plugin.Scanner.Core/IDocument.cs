namespace Plugin.Scanner.Core;

public interface IDocument
{
    IReadOnlyList<IDocumentPage> Pages { get; }
}