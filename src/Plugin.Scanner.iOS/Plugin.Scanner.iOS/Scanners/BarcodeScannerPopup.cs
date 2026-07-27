using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Scanners;

internal sealed class BarcodeScannerPopup : IBarcodeScannerPopup, IDisposable
{
    private DataScannerPopupViewController? _popup;

    private bool _disposedValue;
    private bool _isAttached;

    public void Attach(UIViewController parent, IBarcodeScanOptions options)
    {
        using RecognizedDataType barcodeType = RecognizedDataType.Barcode(options.Formats.ToBarcodeFormats().ToArray());

        _popup = new(
            [barcodeType],
            recognizesMultipleItems: options.RecognizeMultiple,
            isHighlightingEnabled: options.IsHighlightingEnabled,
            isPinchToZoomEnabled: options.IsPinchToZoomEnabled,
            regionOfInterest: options.RegionOfInterest,
            overlay: options.Overlay);
        _popup.Dismissed += Dismissed;

        if (_popup.View is not null)
        {
            parent.AddChildViewController(_popup);
            parent.View?.AddSubview(_popup.View);

            _popup.View.Frame = parent.View?.Bounds ?? CGRect.Empty;
            _popup.View.AutoresizingMask =
                UIViewAutoresizing.FlexibleWidth |
                UIViewAutoresizing.FlexibleHeight;

            _popup.DidMoveToParentViewController(parent);

            _isAttached = true;
        }
    }

    public void Detach()
    {
        _isAttached = false;

        if (_popup?.IsOpen == true)
        {
            _popup?.Dismiss();
        }

        _popup?.Dispose();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dismissed(object? sender, EventArgs e)
    {
        _popup?.Dismissed -= Dismissed;

        Detach();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing
                && _isAttached)
            {
                Detach();
            }

            _disposedValue = true;
        }
    }
}
