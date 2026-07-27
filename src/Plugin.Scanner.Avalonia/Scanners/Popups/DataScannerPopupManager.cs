using System.Diagnostics;
using Avalonia.Controls;
using Plugin.Scanner.Core.Models.Enums;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;

namespace Plugin.Scanner.Avalonia.Scanners.Popups;

internal partial class DataScannerPopupManager : IDataScannerPopupManager
{
    private readonly IBarcodeScannerPopup _barcodeScannerPopup;
    private readonly ITextScannerPopup _textScannerPopup;

    private ScannerType _scannerType;
    private bool _scannerAttached;
    private Control? _attachedControl;
    private TaskCompletionSource? _pageLoadedTcs;

    public DataScannerPopupManager(IBarcodeScannerPopup barcodeScannerPopup, ITextScannerPopup textScannerPopup)
    {
        _barcodeScannerPopup = barcodeScannerPopup;
        _textScannerPopup = textScannerPopup;
    }

    public async Task Attach(Control control, IScanOptions options, CancellationToken cancellationToken)
    {
        if (control.IsLoaded == false)
        {
            // Make sure UI is setup before we try attach the popup
            control.SizeChanged += SizeChanged;

            _pageLoadedTcs = new TaskCompletionSource();

            await _pageLoadedTcs.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

            control.SizeChanged -= SizeChanged;
        }

        control.Unloaded += PageOnUnloaded;

        if (_scannerAttached)
        {
            Trace.TraceWarning("Only one scanner at a time can be attached");
            return;
        }

        _scannerType = options.ScannerType;

        if (_scannerType == ScannerType.Barcode
            && options is IBarcodeScanOptions barcodeOptions)
        {
            AttachBarcodeScanner(control, barcodeOptions);
            _barcodeScannerPopup.Detached += ScannerDetached;
        }
        else if (_scannerType == ScannerType.Text
            && options is ITextScanOptions textScanOptions)
        {
            AttachTextScanner(control, textScanOptions);
            _textScannerPopup.Detached += ScannerDetached;
        }

        _attachedControl = control;
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
            _barcodeScannerPopup.Detached -= ScannerDetached;
            _barcodeScannerPopup.Detach();
        }
        else if (_scannerType == ScannerType.Text)
        {
            _textScannerPopup.Detached -= ScannerDetached;
            _textScannerPopup.Detach();
        }

        _attachedControl?.SetValue(Xaml.DataScannerPopupManager.IsAttachedProperty, false);
        _scannerAttached = false;
    }

    private void ScannerDetached(object? sender, EventArgs e)
    {
        Detach();
    }

    private void PageOnUnloaded(object? sender, EventArgs e)
    {
        Detach();

        _attachedControl?.Unloaded -= PageOnUnloaded;
        _attachedControl = null;
    }

    private void SizeChanged(object? sender, EventArgs e)
    {
        _pageLoadedTcs?.TrySetResult();
    }

    private partial void AttachBarcodeScanner(Control control, IBarcodeScanOptions options);

    private partial void AttachTextScanner(Control control, ITextScanOptions options);
}
