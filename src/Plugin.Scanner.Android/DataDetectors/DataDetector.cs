using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android.DataDetectors;

internal abstract class DataDetector<TDetectedItemsType> : Java.Lang.Object, IDataDetector
    where TDetectedItemsType : class
{
    private readonly IDetector _detector;
    private readonly IRecognizedItemFactory<TDetectedItemsType> _recognizedItemFactory;

    public DataDetector(IDetector detector, IRecognizedItemFactory<TDetectedItemsType> recognizedItemFactory)
    {
        _detector = detector;
        _recognizedItemFactory = recognizedItemFactory;
    }

    public IDetector Detector => _detector;

    public Rect? RegionOfInterest { get; set; }

    public EventHandler<IReadOnlyList<RecognizedItem>>? Detected { get; set; }

    public EventHandler? Cleared { get; set; }

    protected IRecognizedItemFactory<TDetectedItemsType> RecognizedItemFactory => _recognizedItemFactory;

    public abstract void Accept(Java.Lang.Object? t);

    public void Stop()
    {
        _detector.Close();
    }

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