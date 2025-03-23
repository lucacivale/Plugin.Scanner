using System.Diagnostics.CodeAnalysis;
using Plugin.Scanner.iOS.Exceptions;

#pragma warning disable SA1300
namespace Plugin.Scanner.iOS;
#pragma warning restore SA1300

/// <summary>
/// An object that scans the camera live video for text, data in text, and machine-readable codes.
/// </summary>
public sealed class DataScannerViewController : IDisposable
{
    private readonly Plugin.Scanner.iOS.Binding.DataScannerViewController _dataScannerViewController;

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataScannerViewController"/> class.
    /// </summary>
    /// <param name="recognizedDataTypes">The types of data that the data scanner identifies in the live video.</param>
    /// <param name="qualityLevel">The level of resolution to scan that depends on the size of the items.</param>
    /// <param name="recognizesMultipleItems">A Boolean value that indicates whether the scanner identifies all items in the live video.</param>
    /// <param name="isHighFrameRateTrackingEnabled">A Boolean value that determines the frequency that the scanner updates the geometry of recognized items.</param>
    /// <param name="isPinchToZoomEnabled">A Boolean value that indicates whether people can use a two-finger pinch-to-zoom gesture.</param>
    /// <param name="isGuidanceEnabled">A Boolean value that indicates whether the scanner provides help to a person when selecting items.</param>
    /// <param name="isHighlightingEnabled">A Boolean value that indicates whether the scanner displays highlights around recognized items.</param>
    /// <exception cref="ArgumentNullException">Throws if no data type is defined.</exception>
    public DataScannerViewController(
        RecognizedDataType[] recognizedDataTypes,
        QualityLevel qualityLevel = QualityLevel.Balanced,
        bool recognizesMultipleItems = false,
        bool isHighFrameRateTrackingEnabled = true,
        bool isPinchToZoomEnabled = true,
        bool isGuidanceEnabled = true,
        bool isHighlightingEnabled = true)
    {
        _dataScannerViewController = new(
            recognizedDataTypes.Select(x => x.DataType() ?? throw new ArgumentNullException(nameof(x), "No DataType defined.")).ToArray(),
            qualityLevel.ToVnQualityLevel(),
            recognizesMultipleItems,
            isHighFrameRateTrackingEnabled,
            isPinchToZoomEnabled,
            isGuidanceEnabled,
            isHighlightingEnabled);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DataScannerViewController"/> class.
    /// </summary>
    ~DataScannerViewController()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets a value indicating whether the device supports data scanning.
    /// </summary>
    public static bool IsSupported => Plugin.Scanner.iOS.Binding.DataScannerViewController.IsSupported;

    /// <summary>
    /// Gets a value indicating whether a person grants your app access to the camera and doesn’t have any restrictions to using the camera.
    /// </summary>
    public static bool IsAvailable => Plugin.Scanner.iOS.Binding.DataScannerViewController.IsAvailable;

    /// <summary>
    /// Gets the identifiers for the languages that the data scanner recognizes.
    /// </summary>
    public static IEnumerable<string> SupportedTextRecognitionLanguages => Plugin.Scanner.iOS.Binding.DataScannerViewController.SupportedTextRecognitionLanguages;

    /// <summary>
    /// Gets the possible reasons the data scanner is unavailable.
    /// </summary>
    public static IEnumerable<string> ScanningUnavailable => Plugin.Scanner.iOS.Binding.DataScannerViewController.ScanningUnavailable;

    /// <summary>
    /// Gets underlying DataScannerViewController.
    /// </summary>
    public UIViewController ScannerViewController => _dataScannerViewController.ViewController;

    /// <summary>
    /// Gets or sets the delegate that handles user interaction with items recognized by the data scanner.
    /// </summary>
    public DataScannerViewControllerDelegate? Delegate
    {
        get => (DataScannerViewControllerDelegate?)_dataScannerViewController.Delegate;
        set => _dataScannerViewController.Delegate = value;
    }

    /// <summary>
    /// Gets or sets the area of the live video in view coordinates that the data scanner searches for items.
    /// </summary>
    public CGRect RegionOfInterest
    {
        get => _dataScannerViewController.RegionOfInterest;
        set => _dataScannerViewController.RegionOfInterest = value;
    }

    /// <summary>
    /// Gets the minimum zoom factor that the camera supports.
    /// </summary>
    public double MinZoomFactor => _dataScannerViewController.MinZoomFactor;

    /// <summary>
    /// Gets the maximum zoom factor that the camera supports.
    /// </summary>
    public double MaxZoomFactor => _dataScannerViewController.MaxZoomFactor;

    /// <summary>
    /// Gets a value indicating whether the data scanner is actively looking for items.
    /// </summary>
    public bool IsScanning => _dataScannerViewController.IsScanning;

    /// <summary>
    /// Gets the resolution that the scanner uses to find data.
    /// </summary>
    public QualityLevel QualityLevel =>
        _dataScannerViewController.QualityLevel switch
        {
            Plugin.Scanner.iOS.Binding.QualityLevel.Balanced => QualityLevel.Balanced,
            Plugin.Scanner.iOS.Binding.QualityLevel.Accurate => QualityLevel.Accurate,
            Plugin.Scanner.iOS.Binding.QualityLevel.Fast => QualityLevel.Fast,
            _ => QualityLevel.Balanced,
        };

    /// <summary>
    /// Gets a value indicating whether the scanner should identify all items in the live video.
    /// </summary>
    public bool RecognizesMultipleItems => _dataScannerViewController.RecognizesMultipleItems;

    /// <summary>
    /// Gets a value indicating whether the frequency at which the scanner updates the geometry of recognized items.
    /// </summary>
    public bool IsHighFrameRateTrackingEnabled => _dataScannerViewController.IsHighFrameRateTrackingEnabled;

    /// <summary>
    /// Gets a value indicating whether people can use a two-finger pinch-to-zoom gesture.
    /// </summary>
    public bool IsPinchToZoomEnabled => _dataScannerViewController.IsPinchToZoomEnabled;

    /// <summary>
    /// Gets a value indicating whether the scanner provides help to a person when selecting items.
    /// </summary>
    public bool IsGuidanceEnabled => _dataScannerViewController.IsGuidanceEnabled;

    /// <summary>
    /// Gets a value indicating whether the scanner displays highlights around recognized items.
    /// </summary>
    public bool IsHighlightingEnabled => _dataScannerViewController.IsHighlightingEnabled;

    /// <summary>
    /// Gets a view that the data scanner displays over its view without interfering with the Live Text interface.
    /// </summary>
    public UIView OverlayContainerView => _dataScannerViewController.OverlayContainerView;

    /// <summary>
    /// Free resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// An asynchronous array of items that the data scanner currently recognizes in the camera’s live video.
    /// </summary>
    /// <returns>Text items in this array appear in the reading order of the language and region.</returns>
    [SuppressMessage("Performance", "CA1822: Mark members as static", Justification = "False positive.")]
    [SuppressMessage("Performance", "S2325: Methods and properties that don't access instance data should be static", Justification = "False positive.")]
    public Task<RecognizedItem[]> RecognizedItemsAsync()
    {
        TaskCompletionSource<RecognizedItem[]> taskSource = new();

        RecognizedItems(items =>
        {
            List<RecognizedItem> itemList = [];

            foreach (Plugin.Scanner.iOS.Binding.RecognizedItem item in items)
            {
                itemList.Add(item.ToRecognizedItem());
            }

            taskSource.SetResult(itemList.ToArray());
        });

        return taskSource.Task;
    }

    /// <summary>
    /// Captures a high-resolution photo of the camera’s live video.
    /// </summary>
    /// <param name="completionHandler">Completion handler.</param>
    public void CapturePhoto(Action<UIImage?, NSError?> completionHandler)
    {
        _dataScannerViewController.CapturePhoto(completionHandler);
    }

    /// <summary>
    /// Captures a high-resolution photo of the camera’s live video.
    /// </summary>
    /// <returns>An image of the live video.</returns>
    public Task<UIImage> CapturePhotoAsync()
    {
        TaskCompletionSource<UIImage> taskSource = new();

        CapturePhoto((photo, error) =>
        {
            if (photo is null
                || error is not null)
            {
                taskSource.SetException(new DataScannerCapturePhotoException(error?.LocalizedDescription ?? "Something went wrong."));
            }
            else
            {
                taskSource.SetResult(photo);
            }
        });

        return taskSource.Task;
    }

    /// <summary>
    /// Starts scanning the camera’s live video for data.
    /// </summary>
    /// <param name="exception">Start failed.</param>
    public void StartScanning(out DataScannerStartException? exception)
    {
        exception = null;
        _dataScannerViewController.StartScanning(out NSError? error);

        using (error)
        {
            if (error is not null)
            {
                exception = new DataScannerStartException(error.Description);
            }
        }
    }

    private void RecognizedItems(Action<NSArray<Plugin.Scanner.iOS.Binding.RecognizedItem>> completionHandler)
    {
        _dataScannerViewController.RecognizedItems(completionHandler);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        if (disposing)
        {
            _dataScannerViewController.Dispose();
        }
    }
}