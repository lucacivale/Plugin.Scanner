using AndroidX.Camera.MLKit.Vision;
using Java.Interop;
using Java.Util;
using Plugin.Scanner.Android.Factories;
using Plugin.Scanner.Core.Models;
using Xamarin.Google.MLKit.Vision.Interfaces;
using Xamarin.Google.MLKit.Vision.Text;
using MLBarcode = Xamarin.Google.MLKit.Vision.Barcode.Common.Barcode;

namespace Plugin.Scanner.Android.DataDetectors;

internal sealed class TextDataDetector : DataDetector<Text>
{
    public TextDataDetector(IDetector detector, IRecognizedItemFactory<Text> recognizedItemFactory)
        : base(detector, recognizedItemFactory)
    {
    }

    protected override IReadOnlyList<RecognizedItem>? MlKitResultToRecognizedItems(MlKitAnalyzer.Result result)
    {
        Text? text = null;
        result.GetValue(Detector)?.TryJavaCast(out text);

        using (text)
        {
            return RecognizedItemFactory.Create(text);
        }
    }
}