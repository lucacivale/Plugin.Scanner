using Microsoft.Maui.Platform;
using Plugin.Scanner.iOS;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    int _count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        if (_count == 1)
            CounterBtn.Text = $"Clicked {_count} time";
        else
            CounterBtn.Text = $"Clicked {_count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
        var dataScanner = new DataScannerViewController([RecognizedDataType.Text()]);

        await this.ToUIViewController(Handler.MauiContext).PresentViewControllerAsync(dataScanner.ScannerViewController, true);
        dataScanner.StartScanning(out var error);
    }
}