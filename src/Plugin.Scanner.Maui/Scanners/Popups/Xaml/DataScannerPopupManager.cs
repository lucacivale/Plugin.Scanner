using System.Diagnostics;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Maui.Scanners.Popups.Xaml;

public static class DataScannerPopupManager
{
    public static readonly BindableProperty OptionsProperty = BindableProperty.CreateAttached(
        "Options",
        typeof(IScanOptions),
        typeof(Page),
        null,
        propertyChanged: ScanOptionsPropertyChanged);

    public static readonly BindableProperty IsAttachedProperty = BindableProperty.CreateAttached(
        "IsAttched",
        typeof(bool),
        typeof(Page),
        false,
        BindingMode.TwoWay,
        propertyChanged: IsAttachedPropertyChanged);

    public static void SetOptions(BindableObject target, IScanOptions options)
    {
        target.SetValue(OptionsProperty, options);
    }

    public static IScanOptions? GetOptions(BindableObject target)
    {
        return (IScanOptions?)target.GetValue(OptionsProperty);
    }

    public static void SetIsAttached(BindableObject target, bool isOpen)
    {
        target.SetValue(IsAttachedProperty, isOpen);
    }

    public static bool GetIsAttached(BindableObject target)
    {
        return (bool)target.GetValue(IsAttachedProperty);
    }

    private static void IsAttachedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        TryAttachDetach(bindable, (bool)newValue);
    }

    private static void ScanOptionsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        TryAttachDetach(bindable, GetIsAttached(bindable));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "Is event and okay.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Is event and okay.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch all exceptions to prevent crash.")]
    private static async void TryAttachDetach(BindableObject bindable, bool attach)
    {
        if (GetOptions(bindable) is not IScanOptions options)
        {
            Trace.TraceWarning("Scan options can not be null.");
            return;
        }

        try
        {
            IDataScannerPopupManager popupManager = Application.Current!.Handler!.GetRequiredService<IDataScannerPopupManager>();

            if (attach)
            {
                await popupManager.Attach((Page)bindable, options, CancellationToken.None).ConfigureAwait(true);
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
