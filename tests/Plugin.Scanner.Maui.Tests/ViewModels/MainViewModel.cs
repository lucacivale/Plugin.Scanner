using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Models;
using Plugin.Scanner.Overlays.Barcode;
using System.Diagnostics;
using Plugin.Scanner.Core.Extensions;
using Plugin.Scanner.Core.Scanners;
using Plugin.Scanner.Options;

namespace Plugin.Scanner.Maui.Tests.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IBarcodeScanner _barcodeScanner;
    private readonly ITextScanner _textScanner;

    [ObservableProperty]
    private string _barcode = string.Empty;
    
    [ObservableProperty]
    private string _text = string.Empty;

    public MainViewModel(IBarcodeScanner barcodeScanner, ITextScanner textScanner)
    {
        _barcodeScanner = barcodeScanner;
        _textScanner = textScanner;
    }

    [RelayCommand]
    public async Task ScanBarcode()
    {
        try
        {
            BarcodeScanOptions options = new()
            {
                Formats = BarcodeFormat.All,
                IsHighlightingEnabled = true,
                RegionOfInterest = new CenteredRegionOfInterest(250, 200),
            };

            Barcode = (await _barcodeScanner.ScanAsync(options).ConfigureAwait(false)).Value;
        }
        catch(ScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }

    [RelayCommand]
    public async Task ScanText()
    {
        try
        {
            TextScanOptions options = new()
            {
                IsHighlightingEnabled = true,
            };

            Text = (await _textScanner.ScanAsync(options).ConfigureAwait(false)).Value;
        }
        catch(ScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }
}
