using System.Diagnostics;
using Plugin.Scanner.Core.Models.Enums;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;

namespace Plugin.Scanner.Maui.Scanners.Popups;

internal partial class DataScannerPopupManager : IDataScannerPopupManager
{
    private readonly IBarcodeScannerPopup _barcodeScannerPopup;

    private ScannerType _scannerType;
    private bool _scannerAttached;
    private Page? _attachedPage;
    private TaskCompletionSource? _pageLoadedTcs;

    public DataScannerPopupManager(IBarcodeScannerPopup barcodeScannerPopup)
    {
        _barcodeScannerPopup = barcodeScannerPopup;
    }

    public async Task Attach(Page page, IScanOptions options, CancellationToken cancellationToken)
    {
        if (page.IsLoaded == false)
        {
            // Make sure UI is setup before we try attach the popup
            page.SizeChanged += SizeChanged;

            _pageLoadedTcs = new TaskCompletionSource();

            await _pageLoadedTcs.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

            page.SizeChanged -= SizeChanged;
        }

        page.Unloaded += PageOnUnloaded;

        if (page.Handler?.MauiContext is null)
        {
            Trace.TraceError("Maui Context is null");
            return;
        }

        if (_scannerAttached)
        {
            Trace.TraceWarning("Only one scanner at a time can be attached");
            return;
        }

        _scannerType = options.ScannerType;

        if (_scannerType == ScannerType.Barcode
            && options is IBarcodeScanOptions barcodeOptions)
        {
            AttachBarcodeScanner(page, page.Handler.MauiContext, barcodeOptions);
            _barcodeScannerPopup.Detached += BarcodeScannerDetached;
        }

        _attachedPage = page;
        _scannerAttached = true;
    }

    public void Detach()
    {
        if (_scannerAttached == false)
        {
            return;
        }

        if (_scannerType == ScannerType.Barcode)
        {
            _barcodeScannerPopup.Detached -= BarcodeScannerDetached;
            _barcodeScannerPopup.Detach();
        }

        _attachedPage?.SetValue(Xaml.DataScannerPopupManager.IsAttachedProperty, false);
        _scannerAttached = false;
    }

    private void BarcodeScannerDetached(object? sender, EventArgs e)
    {
        Detach();
    }

    private void PageOnUnloaded(object? sender, EventArgs e)
    {
        Detach();

        _attachedPage?.Unloaded -= PageOnUnloaded;
        _attachedPage = null;
    }

    private void SizeChanged(object? sender, EventArgs e)
    {
        _pageLoadedTcs?.TrySetResult();
    }

    private partial void AttachBarcodeScanner(Page page, IMauiContext context, IBarcodeScanOptions options);
}
