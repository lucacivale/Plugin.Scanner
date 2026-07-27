namespace Plugin.Scanner.Core.Scanners.Popups;

public partial interface IScannerPopup<in TOptions>
{
    void Attach(UIViewController parent, TOptions options);
}
