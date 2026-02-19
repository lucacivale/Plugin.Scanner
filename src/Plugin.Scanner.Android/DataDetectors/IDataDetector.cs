using AndroidX.Core.Util;
using Plugin.Scanner.Android.Models;
using Plugin.Scanner.Core;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.DataDetectors;

internal interface IDataDetector : IConsumer
{
    EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    EventHandler? Cleared { get; set; }

    IDetector Detector { get; }

    IRegionOfInterest? RegionOfInterest { get; }

    void Stop();
}
