using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups;

public interface IDataScannerPopupManager
{
    Task Attach(Page page, IScanOptions options, CancellationToken cancellationToken);

    void Detach();
}
