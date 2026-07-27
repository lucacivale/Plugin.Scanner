using System.Diagnostics;
using Android.Runtime;
using Android.Views;
using Avalonia.Platform;
using Plugin.Scanner.Core.Options;
using AvaloniaControl = Avalonia.Controls.Control;
using Object = Java.Lang.Object;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    private partial void AttachBarcodeScanner(IPlatformHandle platformHandle, IBarcodeScanOptions options)
    {
        View? view = Object.GetObject<View>(platformHandle.Handle, JniHandleOwnership.DoNotTransfer);

        if (view is null)
        {
            Trace.TraceWarning("Failed to get view from platform handle");
            return;
        }

        _barcodeScannerPopup.Attach(view, options);
    }

    private partial void AttachTextScanner(IPlatformHandle platformHandle, ITextScanOptions options)
    {
        View? view = Object.GetObject<View>(platformHandle.Handle, JniHandleOwnership.DoNotTransfer);

        if (view is null)
        {
            Trace.TraceWarning("Failed to get view from platform handle");
            return;
        }

        _textScannerPopup.Attach(view, options);
    }
}
