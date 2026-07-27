using AndroidX.Camera.View;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Xamarin.Google.MLKit.Vision.BarCode;

namespace Plugin.Scanner.Android.Scanners.Popups;

internal sealed class BarcodeScannerPopup : ScannerPopup<IBarcodeScanOptions>, IBarcodeScannerPopup
{
    public BarcodeScannerPopup(ICurrentActivity currentActivity)
        : base(currentActivity)
    {
    }

    protected override IDataDetector CreateDataDetector(IBarcodeScanOptions options)
    {
        List<int> formats = options.Formats.ToBarcodeFormats().ToList();

        using BarcodeScannerOptions.Builder builder = new();
        using BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
            .Build();

        return new BarcodeDataDetector(BarcodeScanning.GetClient(scannerOptions), new RecognizedItemFactoryBarcode());
    }

    protected override DataScannerPopup CreateDataScannerPopup(
        Activity activity,
        View parent,
        IDataDetector dataDetector,
        LifecycleCameraController cameraController,
        IBarcodeScanOptions options)
    {
        return new(
            activity,
            parent,
            dataDetector,
            cameraController,
            options.RegionOfInterest,
            options.Overlay,
            options.RecognizeMultiple,
            options.IsHighlightingEnabled);
    }
}
