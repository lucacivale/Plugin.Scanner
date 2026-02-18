using System.Diagnostics.CodeAnalysis;
using Android.OS;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Xamarin.Google.MLKit.Vision.BarCode;
using ASize = Android.Util.Size;
using Exception = System.Exception;
using IBarcodeScanner = Plugin.Scanner.Core.Barcode.IBarcodeScanner;

namespace Plugin.Scanner.Android.Barcode;

/// <summary>
/// Provides Android-specific implementation of the barcode scanner interface using Google ML Kit.
/// </summary>
/// <remarks>
/// <para>
/// This class implements <see cref="Core.Barcode.IBarcodeScanner"/> for Android devices and uses Google ML Kit's
/// barcode scanning capabilities through a camera dialog interface.
/// </para>
/// <para>
/// The scanner presents a full-screen camera interface with visual barcode highlighting and
/// requires the <c>CAMERA</c> permission to be granted.
/// </para>
/// </remarks>
public sealed class BarcodeScanner : IBarcodeScanner
{
    private readonly ICurrentActivity _currentActivity;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScanner"/> class.
    /// </summary>
    /// <param name="currentActivity">The current activity provider for accessing the Android activity context.</param>
    public BarcodeScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    /// <summary>
    /// Asynchronously scans for a barcode using the device camera with ML Kit barcode detection.
    /// </summary>
    /// <param name="options">The scan options specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the scan operation.</param>
    /// <returns>
    /// A task that represents the asynchronous scan operation. The task result contains
    /// the scanned barcode with its decoded value.
    /// </returns>
    /// <exception cref="BarcodeScanException">
    /// Thrown when the scan operation fails due to one of the following reasons:
    /// <list type="bullet">
    /// <item><description>The device has no camera available</description></item>
    /// <item><description>The main executor is not available</description></item>
    /// <item><description>Required UI views cannot be found</description></item>
    /// <item><description>ML Kit analyzer returns an unexpected result type</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    /// Thrown when the operation is canceled via the <paramref name="cancellationToken"/>
    /// or by the user dismissing the scanner dialog.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method creates and displays a <see cref="DataScannerDialog"/> that handles
    /// the camera preview, barcode detection, and user interaction.
    /// </para>
    /// <para>
    /// The dialog is automatically disposed after the scan completes or is canceled.
    /// </para>
    /// </remarks>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        _ = _currentActivity.Activity.MainLooper ?? throw new BarcodeScanException("MainLooper can't be null here");

        TaskCompletionSource<IBarcode> scanCompleteTaskSource = new();

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
                using BarcodeDetector barcodeDetector = new(BarcodeScanning.GetClient(scannerOptions));
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
                    options.RecognizeMultiple,
                    options.IsHighlightingEnabled);

                IBarcode barcode = new Core.Barcode.Barcode((await scannerDialog.ScanAsync(cancellationToken).ConfigureAwait(true)).Text);
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
            throw new BarcodeScanException(e.Message, e);
        }
    }
}