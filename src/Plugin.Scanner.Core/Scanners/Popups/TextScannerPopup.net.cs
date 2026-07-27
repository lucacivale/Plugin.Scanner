using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners.Popups;

internal sealed class TextScannerPopup : ITextScannerPopup
{
    public EventHandler? Detached { get; set; }

    public void Attach(object parent, ITextScanOptions options)
    {
        // Intentionally left empty
    }

    public void Detach()
    {
        // Intentionally left empty
    }
}
