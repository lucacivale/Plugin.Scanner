using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Core;

public partial interface IOverlay : IDisposable
{
    void Cleanup();

    void Detected(IReadOnlyList<RecognizedItem> items);

    void Cleared();
}