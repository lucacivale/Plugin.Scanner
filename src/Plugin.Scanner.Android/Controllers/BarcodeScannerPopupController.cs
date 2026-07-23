using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Controllers;
using Plugin.Scanner.Core.Options;
using Xamarin.Google.MLKit.Vision.BarCode;
using ASize = Android.Util.Size;

namespace Plugin.Scanner.Android.Controllers;

public class BarcodeScannerPopupController : IDataScannerPopupController<IBarcodeScanOptions>
{
    private readonly Dictionary<ViewGroup, DataScannerPopup> _dataScannerPopups;

    public BarcodeScannerPopupController()
    {
        _dataScannerPopups = [];
    }

    public void Add(ViewGroup parent, IBarcodeScanOptions options)
    {
        ArgumentNullException.ThrowIfNull(parent.Context);

        if (parent.Context is not ILifecycleOwner owner)
        {
            throw new ActivityMustBeILifecycleOwnerException("Activity must implement ILifecycleOwner");
        }

        IExecutor mainExecutor = ContextCompat.GetMainExecutor(parent.Context) ?? throw new MainExecutorNotAvailableException("Main executor not available.");

        List<int> formats = options.Formats.ToBarcodeFormats().ToList();

        BarcodeScannerOptions.Builder builder = new();
        BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
            .Build();
        BarcodeDataDetector barcodeDetector = new(BarcodeScanning.GetClient(scannerOptions), new RecognizedItemFactoryBarcode());
        MlKitAnalyzer analyzer = new([barcodeDetector.Detector], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, barcodeDetector);

        LifecycleCameraController cameraController = new(parent.Context);
        cameraController.BindToLifecycle(owner);
        cameraController.SetImageAnalysisAnalyzer(mainExecutor, analyzer);
        cameraController.ImageAnalysisBackpressureStrategy = ImageAnalysis.StrategyKeepOnlyLatest;
        cameraController.PinchToZoomEnabled = options.IsPinchToZoomEnabled;

        // As google recommends https://developers.google.com/ml-kit/vision/barcode-scanning/android?hl=de 2 mp
        ResolutionSelector.Builder resolutionBuilder = new();
        ASize size = new(1920, 1080);
        ResolutionStrategy resolutionStrategy = new(size, ResolutionStrategy.FallbackRuleClosestHigherThenLower);
        AspectRatioStrategy aspectRatioStrategy = new(AspectRatio.Ratio169, AspectRatio.RatioDefault);

        cameraController.ImageAnalysisResolutionSelector = resolutionBuilder
            .SetResolutionStrategy(resolutionStrategy)
            .SetAspectRatioStrategy(aspectRatioStrategy)
            .Build();

        DataScannerPopup popup = new(
            parent.Context,
            barcodeDetector,
            cameraController,
            options.RegionOfInterest,
            options.Overlay,
            options.RecognizeMultiple,
            options.IsHighlightingEnabled);

        _dataScannerPopups.Add(parent, popup);

        popup.Show(parent);
    }

    public void Remove(ViewGroup parent)
    {
        _dataScannerPopups.First(x => ReferenceEquals(x.Key, parent)).Value.Cancel();
    }
}
