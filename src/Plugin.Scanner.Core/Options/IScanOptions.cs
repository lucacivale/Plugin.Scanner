namespace Plugin.Scanner.Core.Options;

public interface IScanOptions
{
    bool RecognizeMultiple { get; }

    bool IsHighlightingEnabled { get; }

    bool IsPinchToZoomEnabled { get; }

    IRegionOfInterest? RegionOfInterest { get; }

    IOverlay Overlay { get; }
}