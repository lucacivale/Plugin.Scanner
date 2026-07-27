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

    public async Task<bool> Attach(Page page, IScanOptions options)
    {
        bool canAttach = true;

        _attachedPage = page;

        if (_attachedPage.IsLoaded == false)
        {
            _attachedPage.Loaded += PageOnLoaded;

            _pageLoadedTcs = new TaskCompletionSource();

            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(1));

            await _pageLoadedTcs.Task.WaitAsync(cts.Token).ConfigureAwait(true);

            _attachedPage.Loaded -= PageOnLoaded;
        }

        page.Unloaded += PageOnUnloaded;

        if (page.Handler?.MauiContext is null)
        {
            Trace.TraceError("Maui Context is null");
            canAttach = false;
        }

        if (_scannerAttached)
        {
            Trace.TraceWarning("Only one scanner at a time can be attached");
            canAttach = false;
        }

        _scannerType = options.ScannerType;

        return canAttach;
    }

    private void PageOnUnloaded(object? sender, EventArgs e)
    {
        Detach();

        _attachedPage?.Unloaded -= PageOnUnloaded;
        _attachedPage = null;
    }

    private void PageOnLoaded(object? sender, EventArgs e)
    {
        _pageLoadedTcs?.TrySetResult();
    }

    public partial Task AttachBarcodeScanner(Page page, IBarcodeScanOptions options);

    public void Detach()
    {
        if (_scannerAttached == false)
        {
            return;
        }

        if (_scannerType == ScannerType.Barcode)
        {
            _barcodeScannerPopup.Detach();
        }

        _attachedPage?.SetValue(Xaml.DataScannerPopupManager.IsBarcodeScannerOpenProperty, false);
        _scannerAttached = false;
    }
}
