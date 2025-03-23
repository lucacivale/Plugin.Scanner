namespace Plugin.Scanner.Maui;

using Core.Barcode;

public partial class MainPage : ContentPage
{
    private readonly IBarcodeScanner _barcodeScanner;
    
    public MainPage(IBarcodeScanner barcodeScanner)
    {
        InitializeComponent();
        
        _barcodeScanner = barcodeScanner;
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        Barcode.Text = await _barcodeScanner.ScanBarcodeAsync(new BarcodeScanOptions(BarcodeFormat.SupportedBarcodeFormats()));
    }
}