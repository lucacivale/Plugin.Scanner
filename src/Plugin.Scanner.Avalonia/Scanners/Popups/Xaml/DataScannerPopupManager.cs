using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Avalonia.Scanners.Popups.Xaml;

public sealed class DataScannerPopupManager : AvaloniaObject
{
    public static readonly AttachedProperty<IScanOptions?> OptionsProperty = AvaloniaProperty.RegisterAttached<DataScannerPopupManager, Control, IScanOptions?>(
        "Options",
        null,
        false);

    public static readonly AttachedProperty<bool> IsAttachedProperty = AvaloniaProperty.RegisterAttached<DataScannerPopupManager, Control, bool>(
        "IsAttched",
        false,
        defaultBindingMode: BindingMode.TwoWay);

    static DataScannerPopupManager()
    {
        IsAttachedProperty.Changed.AddClassHandler<Control>(HandleIsAttachedChanged);
        OptionsProperty.Changed.AddClassHandler<Control>(HandleOptionsChanged);
    }

    public static void SetOptions(AvaloniaObject target, IScanOptions options)
    {
        target.SetValue(OptionsProperty, options);
    }

    public static IScanOptions? GetOptions(AvaloniaObject target)
    {
        return target.GetValue(OptionsProperty);
    }

    public static void SetIsAttached(AvaloniaObject target, bool isOpen)
    {
        target.SetValue(IsAttachedProperty, isOpen);
    }

    public static bool GetIsAttached(AvaloniaObject target)
    {
        return target.GetValue(IsAttachedProperty);
    }

    private static void HandleIsAttachedChanged(Control target, AvaloniaPropertyChangedEventArgs args)
    {
        TryAttachDetach(target, (bool?)args.NewValue == true);
    }

    private static void HandleOptionsChanged(Control target, AvaloniaPropertyChangedEventArgs args)
    {
        TryAttachDetach(target, GetIsAttached(target));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "Is event and okay.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Is event and okay.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch all exceptions to prevent crash.")]
    private static async void TryAttachDetach(AvaloniaObject bindable, bool attach)
    {
        if (GetOptions(bindable) is not IScanOptions options)
        {
            Trace.TraceWarning("Scan options can not be null.");
            return;
        }

        try
        {
            IDataScannerPopupManager popupManager = Avalonia.DataScannerPopupManager.Default;

            if (attach)
            {
                await popupManager.Attach((Control)bindable, options, CancellationToken.None).ConfigureAwait(true);
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
