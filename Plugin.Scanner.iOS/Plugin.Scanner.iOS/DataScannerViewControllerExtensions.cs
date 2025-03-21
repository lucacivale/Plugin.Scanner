#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

internal static class DataScannerViewControllerExtensions
{
    internal static UIView AddBarcodeRegionOfInterestOverlay(this DataScannerViewController dataScannerViewController)
    {
        BarcodeRegionOfInterestOverlay view = new(dataScannerViewController);
        dataScannerViewController.OverlayContainerView.AddSubview(view);

        view.TranslatesAutoresizingMaskIntoConstraints = false;
        NSLayoutConstraint.ActivateConstraints(
        [
            view.HeightAnchor.ConstraintEqualTo(dataScannerViewController.OverlayContainerView.HeightAnchor, multiplier: 0.5f),
            view.CenterXAnchor.ConstraintEqualTo(dataScannerViewController.OverlayContainerView.CenterXAnchor),
            view.CenterYAnchor.ConstraintEqualTo(dataScannerViewController.OverlayContainerView.CenterYAnchor),
            view.LeadingAnchor.ConstraintEqualTo(dataScannerViewController.OverlayContainerView.LeadingAnchor, 25),
            view.TrailingAnchor.ConstraintEqualTo(dataScannerViewController.OverlayContainerView.TrailingAnchor, -25),
        ]);

        return view;
    }
}