using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS.Scanners.Popups;

internal sealed class BarcodeScannerPopup : ScannerPopup<IBarcodeScanOptions>, IBarcodeScannerPopup
{
    protected override DataScannerPopupViewController CreateViewController(IBarcodeScanOptions options)
    {
        using RecognizedDataType barcodeType = RecognizedDataType.Barcode(options.Formats.ToBarcodeFormats().ToArray());

        return new(
            [barcodeType],
            recognizesMultipleItems: options.RecognizeMultiple,
            isHighlightingEnabled: options.IsHighlightingEnabled,
            isPinchToZoomEnabled: options.IsPinchToZoomEnabled,
            regionOfInterest: options.RegionOfInterest,
            overlay: options.Overlay);
    }
}
