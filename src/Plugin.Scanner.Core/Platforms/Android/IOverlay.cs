using Plugin.Scanner.Core.Models;

namespace Plugin.Scanner.Core;

public partial interface IOverlay
{
    void Init(Dialog dialog, View root);

    void Detected(IReadOnlyList<RecognizedItem> items);

    void Cleared();
}