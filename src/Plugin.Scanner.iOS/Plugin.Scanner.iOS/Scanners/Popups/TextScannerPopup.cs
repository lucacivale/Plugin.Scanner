using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Plugin.Scanner.iOS.Binding;

namespace Plugin.Scanner.iOS.Scanners.Popups;

internal sealed class TextScannerPopup : ScannerPopup<ITextScanOptions>, ITextScannerPopup
{
    protected override DataScannerPopupViewController CreateViewController(ITextScanOptions options)
    {
        using RecognizedDataType types = RecognizedDataType.Text(Binding.DataScannerViewController.SupportedTextRecognitionLanguages, TextContentType.Default);
        return new(
            [types],
            recognizesMultipleItems: true,
            isHighlightingEnabled: options.IsHighlightingEnabled,
            isPinchToZoomEnabled: options.IsPinchToZoomEnabled,
            regionOfInterest: options.RegionOfInterest,
            overlay: options.Overlay);
    }
}
