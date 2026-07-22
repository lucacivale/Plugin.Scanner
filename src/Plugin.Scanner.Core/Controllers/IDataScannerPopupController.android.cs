using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Core.Controllers;

public partial interface IDataScannerPopupController<TOptions>
    where TOptions : IScanOptions
{
    void Add(ViewGroup parent, TOptions options);

    void Remove(ViewGroup parent);
}
