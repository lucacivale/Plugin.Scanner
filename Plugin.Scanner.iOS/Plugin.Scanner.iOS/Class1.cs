namespace Plugin.Scanner.iOS;

public class Class1
{
    private Plugin.Scanner.iOS.Binding.DataScannerViewController _a;

    public Class1()
    {
        var a = Plugin.Scanner.iOS.Binding.RecognizedDataType.Text([], Plugin.Scanner.iOS.Binding.TextContentType.Default);
        _a = new([a], Plugin.Scanner.iOS.Binding.QualityLevel.Balanced, false, true, true, true, false);

        var b = 10;
    }
}