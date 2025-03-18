using System.Diagnostics;
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
        var dataScanner = new DataScannerViewController([RecognizedDataType.Text()], recognizesMultipleItems: true);

        dataScanner.StartScanning(out var error);
        
        Task.Delay(5000).ContinueWith(async (task) =>
        {
            var a = await dataScanner.RecognizedItemsAsync();

            foreach (var item in a)
            {
                Debug.WriteLine(item.Value);
            }
        });

        await this.ToUIViewController(Handler.MauiContext).PresentViewControllerAsync(dataScanner.ScannerViewController, true);
        
        var b = 10;
    }
}