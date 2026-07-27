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
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners.Popups;
using Xamarin.Google.MLKit.Vision.BarCode;
using ASize = Android.Util.Size;

namespace Plugin.Scanner.Android.Scanners;

internal sealed class BarcodeScannerPopup : IBarcodeScannerPopup, IDisposable
{
    private readonly ICurrentActivity _currentActivity;

    private BarcodeDataDetector? _barcodeDetector;
    private MlKitAnalyzer? _analyzer;
    private LifecycleCameraController? _cameraController;
    private DataScannerPopup? _popup;

    private bool _disposedValue;
    private bool _isAttached;

    public EventHandler? Detached { get; set; }

    public BarcodeScannerPopup(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    public void Attach(View parent, IBarcodeScanOptions options)
    {
        ArgumentNullException.ThrowIfNull(parent.Context);

        if (parent.Context is not ILifecycleOwner owner)
        {
            throw new ActivityMustBeILifecycleOwnerException("Activity must implement ILifecycleOwner");
        }

        IExecutor mainExecutor = ContextCompat.GetMainExecutor(parent.Context) ?? throw new MainExecutorNotAvailableException("Main executor not available.");

        List<int> formats = options.Formats.ToBarcodeFormats().ToList();

        using BarcodeScannerOptions.Builder builder = new();
        using BarcodeScannerOptions scannerOptions = builder
            .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
            .Build();
        _barcodeDetector = new(BarcodeScanning.GetClient(scannerOptions), new RecognizedItemFactoryBarcode());
        _analyzer = new([_barcodeDetector.Detector], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, _barcodeDetector);

        _cameraController = new(parent.Context);
        _cameraController.BindToLifecycle(owner);
        _cameraController.SetImageAnalysisAnalyzer(mainExecutor, _analyzer);
        _cameraController.ImageAnalysisBackpressureStrategy = ImageAnalysis.StrategyKeepOnlyLatest;
        _cameraController.PinchToZoomEnabled = options.IsPinchToZoomEnabled;

        // As google recommends https://developers.google.com/ml-kit/vision/barcode-scanning/android?hl=de 2 mp
        using ResolutionSelector.Builder resolutionBuilder = new();
        using ASize size = new(1920, 1080);
        using ResolutionStrategy resolutionStrategy = new(size, ResolutionStrategy.FallbackRuleClosestHigherThenLower);
        using AspectRatioStrategy aspectRatioStrategy = new(AspectRatio.Ratio169, AspectRatio.RatioDefault);

        _cameraController.ImageAnalysisResolutionSelector = resolutionBuilder
            .SetResolutionStrategy(resolutionStrategy)
            .SetAspectRatioStrategy(aspectRatioStrategy)
            .Build();

        _popup = new(
            _currentActivity.Activity,
            parent,
            _barcodeDetector,
            _cameraController,
            options.RegionOfInterest,
            options.Overlay,
            options.RecognizeMultiple,
            options.IsHighlightingEnabled);

        _popup.DismissEvent += Popup_DismissEvent;
        _popup.Show();

        _isAttached = true;
    }

    public void Detach()
    {
        if (_isAttached == false)
        {
            return;
        }

        _isAttached = false;

        if (_popup?.IsRunning == true)
        {
            _cameraController?.Unbind();
            _popup.Dismiss();
        }

        _barcodeDetector?.Dispose();
        _analyzer?.Dispose();
        _cameraController?.Dispose();

        _popup?.Dispose();

        Detached?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async void Popup_DismissEvent(object? sender, EventArgs e)
    {
        _popup?.DismissEvent -= Popup_DismissEvent;

        // Wait here because MLKit won't stop analyzing until the pipeline is finished.
        // If we dispose of the dialog before MLKit finishes, it will throw an exception.
        await Task.Delay(450).ConfigureAwait(true);

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
