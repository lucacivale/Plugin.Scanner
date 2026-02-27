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
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Core.Options;
using Plugin.Scanner.Core.Scanners;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;
using ASize = Android.Util.Size;

namespace Plugin.Scanner.Android.Scanners;

internal sealed class TextScanner : ITextScanner
{
    private readonly ICurrentActivity _currentActivity;

    public TextScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }


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

                using DataScannerDialog scannerDialog = new(
                    _currentActivity.Activity,
                    textDetector,
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