using System.Diagnostics;
using Avalonia.Platform;
using ObjCRuntime;
using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

internal partial class DataScannerPopupManager
{
    private partial void AttachBarcodeScanner(IPlatformHandle platformHandle, IBarcodeScanOptions options)
    {
        UIViewController? viewController = FindViewController(Runtime.GetNSObject<UIView>(platformHandle.Handle));

        if (viewController is null)
        {
            Trace.TraceError("Could not find view controller!");
            return;
        }

        _barcodeScannerPopup.Attach(viewController, options);
    }

    private partial void AttachTextScanner(IPlatformHandle platformHandle, ITextScanOptions options)
    {
        UIViewController? viewController = FindViewController(Runtime.GetNSObject<UIView>(platformHandle.Handle));

        if (viewController is null)
        {
            Trace.TraceError("Could not find view controller!");
            return;
        }

        _textScannerPopup.Attach(viewController, options);
    }

    private static UIViewController? FindViewController(UIView? view)
    {
        UIViewController? viewController = null;
        UIResponder? responder = view;

        while (responder is not null)
        {
            responder = responder.NextResponder;

            if (responder is not UIViewController currentViewController)
            {
                continue;
            }

            viewController = currentViewController;
            break;
        }

        return viewController;
    }
}
