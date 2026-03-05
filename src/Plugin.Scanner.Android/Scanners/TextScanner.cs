using Android.OS;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ImageCaptures;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.MLKit.Vision;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using Java.Util.Concurrent;
using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Dialogs;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;
using ASize = Android.Util.Size;

namespace Plugin.Scanner.Android.Scanners;

/// <summary>
/// Provides Android-specific text recognition (OCR) using Google ML Kit.
/// </summary>
internal sealed class TextScanner : ITextScanner
{
    private readonly ICurrentActivity _currentActivity;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextScanner"/> class.
    /// </summary>
    /// <param name="currentActivity">The current activity provider.</param>
    public TextScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    /// <summary>
    /// Scans for text using the device camera with ML Kit text recognition.
    /// </summary>
    /// <param name="options">The text scan configuration options.</param>
    /// <param name="cancellationToken">A token to cancel the scan operation.</param>
    /// <returns>A task containing the recognized text result.</returns>
    /// <exception cref="ScanException">Thrown when the scan operation fails.</exception>
    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IScanResult> ScanAsync(ITextScanOptions options, CancellationToken cancellationToken)
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

                using TextDataDetector textDetector = new(TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions), new RecognizedItemFactoryText());
                using MlKitAnalyzer analyzer = new([textDetector.Detector], ImageAnalysis.CoordinateSystemViewReferenced, mainExecutor, textDetector);

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

                using CameraScannerDialog scannerDialog = new(
                    _currentActivity.Activity,
                    textDetector,
                    cameraController,
                    options.RegionOfInterest,
                    options.Overlay,
                    true,
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

    [SuppressMessage("Usage", "VSTHRD101:Avoid unsupported async delegates", Justification = "We have to await this async call because we have to dispatch to the main queue.")]
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Intentionally catching all exceptions here to prevent background task from crashing the process.")]
    public async Task<IScanResult> ScanAsync(byte[] image, ITextScanOptions options, CancellationToken cancellationToken)
    {
        _ = _currentActivity.Activity.MainLooper ?? throw new ScanException("MainLooper can't be null here");

        TaskCompletionSource<IScanResult> scanCompleteTaskSource = new();

        using Handler handler = new(_currentActivity.Activity.MainLooper);

        handler.Post(async () =>
        {
            try
            {
                using TextDataDetector textDetector = new(TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions), new RecognizedItemFactoryText());

                using Bitmap imageBitmap = await BitmapFactory.DecodeByteArrayAsync(image, 0, image.Length).ConfigureAwait(true) ?? throw new NullReferenceException("Could not decode byte array.");

                using ImageScannerDialog scannerDialog = new(
                    _currentActivity.Activity,
                    imageBitmap,
                    textDetector,
                    options.RegionOfInterest,
                    options.Overlay,
                    true,
                    options.IsHighlightingEnabled);

                using InputImage inputImage = InputImage.FromBitmap(imageBitmap, 0);

                EventHandler @event = null!;
                @event = (_, _) =>
                {
                    textDetector.Process(inputImage);
                    scannerDialog.ShowEvent -= @event;
                };

                scannerDialog.ShowEvent += @event;

                IScanResult barcode = new ScanResult((await scannerDialog.ScanAsync(cancellationToken).ConfigureAwait(true)).Text);
                scanCompleteTaskSource.TrySetResult(barcode);
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