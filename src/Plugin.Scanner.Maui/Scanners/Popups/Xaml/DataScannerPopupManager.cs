using System.Diagnostics;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups.Xaml;

public static class DataScannerPopupManager
{
    public static readonly BindableProperty BarcodeScanOptionsProperty = BindableProperty.CreateAttached(
        "BarcodeScanOptions",
        typeof(IBarcodeScanOptions),
        typeof(Page),
        null,
        propertyChanged: BarcodeScanOptionsPropertyChanged);

    public static readonly BindableProperty IsBarcodeScannerOpenProperty = BindableProperty.CreateAttached(
        "IsBarcodeScannerOpen",
        typeof(bool),
        typeof(Page),
        false,
        propertyChanged: IsBarcodeScannerOpenPropertyChanged);

    public static void SetBarcodeScanOptions(BindableObject target, IBarcodeScanOptions options)
    {
        target.SetValue(BarcodeScanOptionsProperty, options);
    }

    public static IBarcodeScanOptions? GetBarcodeScanOptions(BindableObject target)
    {
        return (IBarcodeScanOptions?)target.GetValue(BarcodeScanOptionsProperty);
    }

    public static void SetIsBarcodeScannerOpen(BindableObject target, bool isOpen)
    {
        target.SetValue(IsBarcodeScannerOpenProperty, isOpen);
    }

    public static bool GetIsBarcodeScannerOpen(BindableObject target)
    {
        return (bool)target.GetValue(IsBarcodeScannerOpenProperty);
    }

    private static void IsBarcodeScannerOpenPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        TryAttachDetach(bindable, (bool)newValue);
    }

    private static void BarcodeScanOptionsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        TryAttachDetach(bindable, GetIsBarcodeScannerOpen(bindable));
    }

    private static async void TryAttachDetach(BindableObject bindable, bool attach)
    {
        if (GetBarcodeScanOptions(bindable) is not IBarcodeScanOptions barcodeScanOptions)
        {
            Trace.TraceWarning("Barcode scan options can not be null.");
            return;
        }

        try
        {
            IDataScannerPopupManager popupManager = Application.Current!.Handler!.GetRequiredService<IDataScannerPopupManager>();

            if (attach)
            {
                await popupManager.AttachBarcodeScanner((Page)bindable, barcodeScanOptions).ConfigureAwait(true);
            }
            else
            {
                popupManager.Detach();
            }
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }
}
