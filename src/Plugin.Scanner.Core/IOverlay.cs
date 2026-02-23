namespace Plugin.Scanner.Core;

public partial interface IOverlay : IDisposable
{
    void AddOverlay();

    void AddRegionOfInterest(IRegionOfInterest? regionOfInterest);

    void Cleanup();
}