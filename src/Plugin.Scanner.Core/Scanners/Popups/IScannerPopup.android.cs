namespace Plugin.Scanner.Core.Scanners.Popups;

public partial interface IScannerPopup<in TOptions>
{
    void Attach(View parent, TOptions options);
}
