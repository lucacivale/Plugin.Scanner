using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;
using Xamarin.Google.MLKit.Vision.Text;

namespace Plugin.Scanner.Android.DataDetectors;

internal sealed class TextDataDetector : DataDetector<Text>
{
    public TextDataDetector(IDetector detector, IRecognizedItemFactory<Text> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
    }

    public override void Accept(Java.Lang.Object? t)
    {
        if (t is not MlKitAnalyzer.Result result)
        {
            return;
        }

        using (result)
        {
            Text? text = null;
            result.GetValue(Detector)?.TryJavaCast(out text);

            using (text)
            {
                ProcessResults(RecognizedItemFactory.Create(text));
            }
        }
    }

    private void ProcessResults(IReadOnlyList<RecognizedItem>? recognizedItems)
    {
        if (recognizedItems is null
            || recognizedItems.Count == 0)
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            IReadOnlyList<RecognizedItem> frequentItems = recognizedItems;

            if (RegionOfInterest is not null)
            {
                frequentItems = recognizedItems
                    .Where(kv => RegionOfInterest.Contains(kv.Bounds.ToRect()))
                    .ToList();

                if (frequentItems.Count == 0)
                {
                    Cleared?.Invoke(this, EventArgs.Empty);
                }
            }

            if (frequentItems.Count != 0)
            {
                Detected?.Invoke(this, frequentItems);
            }
        }
    }
}