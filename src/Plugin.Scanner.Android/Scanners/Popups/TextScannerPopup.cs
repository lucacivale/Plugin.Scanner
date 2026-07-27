using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;

namespace Plugin.Scanner.Android.Scanners.Popups;

internal sealed class TextScannerPopup : ScannerPopup<ITextScanOptions>, ITextScannerPopup
{
    public TextScannerPopup(ICurrentActivity currentActivity)
        : base(currentActivity)
    {
    }

    protected override IDataDetector CreateDataDetector(ITextScanOptions options)
    {
        return new TextDataDetector(TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions), new RecognizedItemFactoryText());
    }

    protected override DataScannerPopup CreateDataScannerPopup(
        Activity activity,
        View parent,
        IDataDetector dataDetector,
        LifecycleCameraController cameraController,
        ITextScanOptions options)
    {
        return new(
            activity,
            parent,
            dataDetector,
            cameraController,
            options.RegionOfInterest,
            options.Overlay,
            true,
            options.IsHighlightingEnabled);
    }
}
