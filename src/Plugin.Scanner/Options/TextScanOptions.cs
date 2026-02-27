using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Overlays.Text;

namespace Plugin.Scanner.Options;

public sealed class TextScanOptions : ITextScanOptions
{
    public bool IsHighlightingEnabled { get; set; } = true;

    public bool IsPinchToZoomEnabled { get; set; } = true;

    public IRegionOfInterest? RegionOfInterest { get; set; }

    public IOverlay Overlay { get; set; } = new TextScannerOverlay();

    public void Dispose()
    {
        Overlay.Dispose();
    }
}