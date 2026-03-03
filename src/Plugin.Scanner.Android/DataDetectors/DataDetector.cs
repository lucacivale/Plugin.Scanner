using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.DataDetectors;

/// <summary>
/// Provides a base implementation for data detectors that process ML Kit vision results.
/// </summary>
/// <typeparam name="TDetectedItemsType">The type of items detected by the ML Kit detector.</typeparam>
internal abstract class DataDetector<TDetectedItemsType> : Java.Lang.Object, IDataDetector
    where TDetectedItemsType : class
{
    private readonly IDetector _detector;
    private readonly IRecognizedItemFactory<TDetectedItemsType> _recognizedItemFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataDetector{TDetectedItemsType}"/> class.
    /// </summary>
    /// <param name="detector">The ML Kit detector instance.</param>
    /// <param name="recognizedItemFactory">The factory for creating recognized items from detection results.</param>
    protected DataDetector(IDetector detector, IRecognizedItemFactory<TDetectedItemsType> recognizedItemFactory)
    {
        _detector = detector;
        _recognizedItemFactory = recognizedItemFactory;
    }

    /// <summary>
    /// Gets the underlying ML Kit detector instance.
    /// </summary>
    public IDetector Detector => _detector;

    /// <summary>
    /// Gets or sets the region of interest for limiting detection area.
    /// </summary>
    public Rect? RegionOfInterest { get; set; }

    /// <summary>
    /// Gets or sets when items are detected in the camera frame.
    /// </summary>
    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    /// <summary>
    /// Gets or sets when the detection area is cleared with no items found.
    /// </summary>
    public EventHandler? Cleared { get; set; }

    /// <summary>
    /// Gets the factory for creating recognized items from detection results.
    /// </summary>
    protected IRecognizedItemFactory<TDetectedItemsType> RecognizedItemFactory => _recognizedItemFactory;

    /// <summary>
    /// Processes the ML Kit analyzer result.
    /// </summary>
    /// <param name="t">The analyzer result object.</param>
    public abstract void Accept(Java.Lang.Object? t);

    /// <summary>
    /// Stops the detector and closes associated resources.
    /// </summary>
    public void Stop()
    {
        _detector.Close();
    }

    /// <summary>
    /// Releases the managed and unmanaged resources used by the detector.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _detector.Close();
            _detector.Dispose();
        }

        base.Dispose(disposing);
    }
}