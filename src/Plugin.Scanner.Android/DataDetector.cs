using Xamarin.Google.MLKit.Vision.Interfaces;

namespace Plugin.Scanner.Android;

internal abstract class DataDetector : Java.Lang.Object
{
    private readonly IDetector _detector;

    protected DataDetector(IDetector detector)
    {
        _detector = detector;
    }

    public IDetector Detector => _detector;

    public EventHandler<(IReadOnlyList<RecognizedItem> Added, IReadOnlyList<RecognizedItem> All)>? Added { get; set; }

    public EventHandler<(IReadOnlyList<RecognizedItem> Removed, IReadOnlyList<RecognizedItem> All)>? Removed { get; set; }

    public EventHandler<(IReadOnlyList<RecognizedItem> Updated, IReadOnlyList<RecognizedItem> All)>? Updated { get; set; }

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