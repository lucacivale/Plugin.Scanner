// ReSharper disable once CheckNamespace
namespace Plugin.Scanner.Core;

public partial interface IOverlay
{
    void Init(Dialog dialog, View root);

    void AddOverlay();

    void AddRegionOfInterest(IRegionOfInterest? regionOfInterest);
}