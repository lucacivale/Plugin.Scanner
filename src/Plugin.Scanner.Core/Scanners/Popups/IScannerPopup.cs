namespace Plugin.Scanner.Core.Scanners.Popups;

public partial interface IScannerPopup<in TParent, in TOptions>
{
    void Attach(TParent parent, TOptions options);

    void Detach();
}
