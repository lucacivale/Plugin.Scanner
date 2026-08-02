using Plugin.Scanner.Core;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Views;

namespace Plugin.Scanner.iOS;

public class DataScannerPopupViewController : UIViewController
{
    private readonly DataScannerPopupView _popupView = new();
    private readonly DataScannerViewController _dataScannerViewController;

    private bool _isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerPopupViewController"/> class.
    /// </summary>
    /// <param name="recognizedDataTypes">Types of data to recognize during scanning.</param>
    /// <param name="qualityLevel">Quality level for recognition.</param>
    /// <param name="recognizesMultipleItems">Whether to recognize multiple items simultaneously.</param>
    /// <param name="isHighFrameRateTrackingEnabled">Whether high frame rate tracking is enabled.</param>
    /// <param name="isPinchToZoomEnabled">Whether pinch-to-zoom is enabled.</param>
    /// <param name="isGuidanceEnabled">Whether user guidance is enabled.</param>
    /// <param name="isHighlightingEnabled">Whether highlighting of recognized items is enabled.</param>
    /// <param name="regionOfInterest">Optional region of interest for scanning.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    public DataScannerPopupViewController(
        RecognizedDataType[] recognizedDataTypes,
        QualityLevel qualityLevel = QualityLevel.Balanced,
        bool recognizesMultipleItems = false,
        bool isHighFrameRateTrackingEnabled = true,
        bool isPinchToZoomEnabled = true,
        bool isGuidanceEnabled = true,
        bool isHighlightingEnabled = true,
        IRegionOfInterest? regionOfInterest = null,
        IOverlay? overlay = null)
    {
        _dataScannerViewController = new(
            recognizedDataTypes,
            qualityLevel,
            recognizesMultipleItems,
            isHighFrameRateTrackingEnabled,
            isPinchToZoomEnabled,
            isGuidanceEnabled,
            isHighlightingEnabled,
            regionOfInterest,
            overlay);
    }

    public EventHandler? Dismissed { get; set; }

    public bool IsOpen { get; set; }

    public override void LoadView()
    {
        View = new PassThroughView();
    }

    public override void DidMoveToParentViewController(UIViewController? parent)
    {
        base.DidMoveToParentViewController(parent);

        IsOpen = true;
    }

    public override void RemoveFromParentViewController()
    {
        base.RemoveFromParentViewController();

        IsOpen = false;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View?.AddSubview(_popupView);

        AddChildViewController(_dataScannerViewController);

        if (_dataScannerViewController.View is not null)
        {
            _popupView.AddSubview(_dataScannerViewController.View);

            _dataScannerViewController.View.AutoresizingMask =
                UIViewAutoresizing.FlexibleWidth |
                UIViewAutoresizing.FlexibleHeight;
            _dataScannerViewController.DidMoveToParentViewController(this);

            if (Binding.DataScannerViewController.IsAvailable
                && Binding.DataScannerViewController.IsSupported)
            {
                _dataScannerViewController.StartScanning();
            }
        }
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        UIView.Animate(0.25, () => _popupView.Alpha = 1);
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();

        _popupView.Frame = CalculatePanelRect();

        _dataScannerViewController.View?.Frame = _popupView.Bounds;
    }

    public void Dismiss()
    {
        UIView.Animate(
            duration: 0.25,
            animation: () =>
            {
                _popupView.Alpha = 0;
            },
            completion: () =>
            {
                WillMoveToParentViewController(null);

                _dataScannerViewController.StopScanning();
                _dataScannerViewController.View?.RemoveFromSuperview();
                _dataScannerViewController.RemoveFromParentViewController();

                _popupView.RemoveFromSuperview();

                View?.RemoveFromSuperview();
                RemoveFromParentViewController();

                Dismissed?.Invoke(this, EventArgs.Empty);
            });
    }

    public void Expand()
    {
        ExpandMinimize(true);
    }

    public void Minimize()
    {
        ExpandMinimize(false);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _dataScannerViewController.Dispose();
            _popupView.Dispose();
        }
    }

    private void ExpandMinimize(bool expand)
    {
        _isExpanded = expand;

        UIView.Animate(
            duration: 0.3,
            animation: () =>
            {
                View?.SetNeedsLayout();
                View?.LayoutIfNeeded();
            });
    }

    private CGRect CalculatePanelRect()
    {
        if (View is null)
        {
            return CGRect.Empty;
        }

        CGRect safe = View.SafeAreaLayoutGuide.LayoutFrame;
        bool isLandscape = safe.Width > safe.Height;

        nfloat width = safe.Width * 2 / 3;
        nfloat height = safe.Height / 3;
        nfloat x = safe.Right - width;
        nfloat y = safe.Top;

        if (isLandscape)
        {
            width = safe.Width / 3;
            height = safe.Height * 2 / 3;
            x = safe.Right - width;
        }

        if (_isExpanded)
        {
            if (isLandscape)
            {
                width = safe.Width / 2;
                height = safe.Height;
                x = safe.Right - width;
            }
            else
            {
                width = safe.Width;
                height = safe.Height / 2;
                x = safe.Left;
            }
        }

        return new CGRect(x, y, width, height);
    }
}
