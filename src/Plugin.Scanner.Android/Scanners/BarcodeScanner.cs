using System.Diagnostics.CodeAnalysis;
using Android.OS;
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
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Options;
using Xamarin.Google.MLKit.Vision.BarCode;
using ASize = Android.Util.Size;
using Exception = System.Exception;
using IBarcodeScanner = Plugin.Scanner.Core.Scanners.IBarcodeScanner;

namespace Plugin.Scanner.Android.Scanners;

/// <summary>
/// Provides Android-specific barcode scanning using Google ML Kit.
/// </summary>
internal sealed class BarcodeScanner : IBarcodeScanner
{
    private readonly ICurrentActivity _currentActivity;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScanner"/> class.
    /// </summary>
    /// <param name="currentActivity">The current activity provider.</param>
    public BarcodeScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    /// <summary>
    /// Scans for barcodes using the device camera with ML Kit barcode detection.
    /// </summary>
    /// <param name="options">The barcode scan configuration options.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task containing the scanned barcode result.</returns>
    /// <exception cref="ScanException">Thrown when the scan operation fails.</exception>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IScanResult> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        _ = _currentActivity.Activity.MainLooper ?? throw new ScanException("MainLooper can't be null here");

        TaskCompletionSource<IScanResult> scanCompleteTaskSource = new();

        using Handler handler = new(_currentActivity.Activity.MainLooper);

        handler.Post(async () =>
        {
            try
            {
                if (_currentActivity.Activity is not ILifecycleOwner owner)
                {
                    throw new ActivityMustBeILifecycleOwnerException("Activity must implement ILifecycleOwner");
                }

                IExecutor mainExecutor = ContextCompat.GetMainExecutor(_currentActivity.Activity) ?? throw new MainExecutorNotAvailableException("Main executor not available.");
                List<int> formats = options.Formats.ToBarcodeFormats().ToList();

                using BarcodeScannerOptions.Builder builder = new();
                using BarcodeScannerOptions scannerOptions = builder
                    .SetBarcodeFormats(formats[0], formats.Skip(1).ToArray())
                    .Build();
                using BarcodeDataDetector barcodeDetector = new(BarcodeScanning.GetClient(scannerOptions), new RecognizedItemFactoryBarcode());
                using MlKitAnalyzer analyzer = new([barcodeDetector.Detector], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, barcodeDetector);

                using LifecycleCameraController cameraController = new(_currentActivity.Activity);
                cameraController.BindToLifecycle(owner);
                cameraController.SetImageAnalysisAnalyzer(mainExecutor, analyzer);
                cameraController.ImageAnalysisBackpressureStrategy = ImageAnalysis.StrategyKeepOnlyLatest;
                cameraController.PinchToZoomEnabled = options.IsPinchToZoomEnabled;

                // As google recommends https://developers.google.com/ml-kit/vision/barcode-scanning/android?hl=de 2 mp
                using ResolutionSelector.Builder resolutionBuilder = new();
                using ResolutionStrategy resolutionStrategy = new(new ASize(1920, 1080), ResolutionStrategy.FallbackRuleClosestHigherThenLower);
                using AspectRatioStrategy aspectRatioStrategy = new(AspectRatio.Ratio169, AspectRatio.RatioDefault);

                cameraController.ImageAnalysisResolutionSelector = resolutionBuilder
                    .SetResolutionStrategy(resolutionStrategy)
                    .SetAspectRatioStrategy(aspectRatioStrategy)
                    .Build();

                using DataScannerDialog scannerDialog = new(
                    _currentActivity.Activity,
                    barcodeDetector,
                    cameraController,
                    options.RegionOfInterest,
                    options.Overlay,
                    options.RecognizeMultiple,
                    options.IsHighlightingEnabled);

                IScanResult barcode = new ScanResult((await scannerDialog.ScanAsync(cancellationToken).ConfigureAwait(true)).Text);
                scanCompleteTaskSource.TrySetResult(barcode);

                cameraController.ClearImageAnalysisAnalyzer();
                cameraController.Unbind();
            }
            catch (Exception e)
            {
                scanCompleteTaskSource.TrySetException(e);
            }
        });

        try
        {
            return await scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception e)
            when (e is MainExecutorNotAvailableException
                or NoCameraException
                or ViewNotFoundException)
        {
            throw new ScanException(e.Message, e);
        }
    }
}