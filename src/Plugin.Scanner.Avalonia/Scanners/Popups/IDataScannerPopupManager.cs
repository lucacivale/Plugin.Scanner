using Avalonia.Controls;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

public interface IDataScannerPopupManager
{
    Task Attach(Control control, IScanOptions options, CancellationToken cancellationToken);

    void Detach();
}
