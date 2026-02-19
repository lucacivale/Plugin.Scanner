using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Models;
using System.Diagnostics;

namespace Plugin.Scanner.Maui.Tests.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IBarcodeScanner _barcodeScanner;

    [ObservableProperty]
    private string _barcode = string.Empty;

    public MainViewModel(IBarcodeScanner barcodeScanner)
    {
        _barcodeScanner = barcodeScanner;
    }

    [RelayCommand]
    public async Task ScanBarcode()
    {
        try
        {
            BarcodeScanOptions options = new()
            {
                Formats = BarcodeFormat.All,
                RecognizeMultiple = true,
                IsHighlightingEnabled = true,
                RegionOfInterest = new CenteredRegionOfInterest(250, 200),
            };
            Barcode = (await _barcodeScanner.ScanAsync(options).ConfigureAwait(false)).RawValue;
        }
        catch(BarcodeScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }
}
