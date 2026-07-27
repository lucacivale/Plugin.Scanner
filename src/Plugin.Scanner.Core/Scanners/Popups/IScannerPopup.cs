namespace Plugin.Scanner.Core.Scanners.Popups;

public partial interface IScannerPopup<in TOptions>
{
    EventHandler? Detached { get; set; }

    void Detach();
}
