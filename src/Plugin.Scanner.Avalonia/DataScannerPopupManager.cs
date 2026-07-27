using Plugin.Scanner.Avalonia.Scanners.Popups;

namespace Plugin.Scanner.Avalonia;

public static class DataScannerPopupManager
{
    private static IDataScannerPopupManager? _dataScannerPopupManagerImplementation;

    public static IDataScannerPopupManager Default => _dataScannerPopupManagerImplementation ??= new Scanners.Popups.DataScannerPopupManager(BarcodeScannerPopup.Default, TextScannerPopup.Default);
}
