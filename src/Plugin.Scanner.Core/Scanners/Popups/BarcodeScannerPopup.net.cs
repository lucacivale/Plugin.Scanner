using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Scanners.Popups;

internal sealed class BarcodeScannerPopup : IBarcodeScannerPopup
{
    public EventHandler? Detached { get; set; }

    public void Attach(object parent, IBarcodeScanOptions options)
    {
        // Intentionally left empty
    }

    public void Detach()
    {
        // Intentionally left empty
    }
}
