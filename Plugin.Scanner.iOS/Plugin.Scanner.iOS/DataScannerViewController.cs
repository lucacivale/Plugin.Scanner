namespace Plugin.Scanner.iOS;

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

    ~DataScannerViewController()
    {
        Dispose(false);
    }

    /// <summary>
    /// Underlying DataScannerViewController.
    /// </summary>
    public UIViewController ScannerViewController => _dataScannerViewController.ViewController;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Starts scanning the camera’s live video for data.
    /// </summary>
    /// <param name="error">Start failed.</param>
    public void StartScanning(out NSError? error)
    {
        _dataScannerViewController.StartScanning(out error);
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