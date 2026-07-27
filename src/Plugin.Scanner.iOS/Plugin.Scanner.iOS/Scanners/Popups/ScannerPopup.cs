using Plugin.Scanner.Core.Options;

namespace Plugin.Scanner.iOS.Scanners.Popups;

internal abstract class ScannerPopup<TOptions> : IDisposable
    where TOptions : IScanOptions
{
    private DataScannerPopupViewController? _popup;

    private bool _disposedValue;
    private bool _isAttached;

    public EventHandler? Detached { get; set; }

    public void Attach(UIViewController parent, TOptions options)
    {
        _popup = CreateViewController(options);
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
        if (_isAttached == false)
        {
            return;
        }

        _isAttached = false;

        if (_popup?.IsOpen == true)
        {
            _popup?.Dismiss();
        }

        _popup?.Dispose();

        Detached?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract DataScannerPopupViewController CreateViewController(TOptions options);

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
