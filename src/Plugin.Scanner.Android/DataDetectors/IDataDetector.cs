using AndroidX.Core.Util;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.DataDetectors;

/// <summary>
/// Defines a contract for data detectors that process camera frames and recognize items.
/// </summary>
internal interface IDataDetector : IConsumer
{
    /// <summary>
    /// Gets or sets when items are detected in the camera frame.
    /// </summary>
    EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    /// <summary>
    /// Gets or sets  when the detection area is cleared with no items found.
    /// </summary>
    EventHandler? Cleared { get; set; }

    /// <summary>
    /// Gets the underlying ML Kit detector instance.
    /// </summary>
    IDetector Detector { get; }

    /// <summary>
    /// Gets or sets the region of interest for limiting detection area.
    /// </summary>
    Rect? RegionOfInterest { get; set; }

    /// <summary>
    /// Stops the detector and releases associated resources.
    /// </summary>
    void Stop();
}
