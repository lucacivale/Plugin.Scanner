using System.Diagnostics;
using Microsoft.Maui.Platform;
using Plugin.Scanner.iOS;
using Plugin.Scanner.iOS.Barcode;

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

        var result = await BarcodeScanner.ScanBarcodeAsync();

        var a = 10;
    }
}