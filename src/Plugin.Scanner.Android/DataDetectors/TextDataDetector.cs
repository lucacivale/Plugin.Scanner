using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;
using Xamarin.Google.MLKit.Vision.Text;

namespace Plugin.Scanner.Android.DataDetectors;

/// <summary>
/// Detects text in camera frames using ML Kit text recognition.
/// </summary>
internal sealed class TextDataDetector : DataDetector<Text>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextDataDetector"/> class.
    /// </summary>
    /// <param name="detector">The ML Kit text detector instance.</param>
    /// <param name="recognizedItemFactory">The factory for creating recognized text items.</param>
    public TextDataDetector(IDetector detector, IRecognizedItemFactory<Text> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
    }

    /// <summary>
    /// Processes the ML Kit analyzer result and extracts recognized text.
    /// </summary>
    /// <param name="t">The analyzer result object.</param>
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

    /// <summary>
    /// Processes the recognized text items and raises appropriate events based on detection results.
    /// </summary>
    /// <param name="recognizedItems">The list of recognized text items.</param>
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