using Plugin.Scanner.Core.Barcode;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object? sender, EventArgs e)
    {
        try
        {
            IBarcode a = await Handler.GetRequiredService<IBarcodeScanner>().ScanBarcodeAsync(new BarcodeScanOptions() { Formats = BarcodeFormat.All});
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}