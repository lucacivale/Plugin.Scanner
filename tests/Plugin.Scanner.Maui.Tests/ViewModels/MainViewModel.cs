using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
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
            DisplayInfo display = DeviceDisplay.Current.MainDisplayInfo;

            double screenWidth = display.Width;     // raw pixels
            double screenHeight = display.Height;   // raw pixels

            Plugin.Scanner.Core.Models.Rect centeredRect = new(
                Convert.ToInt32((screenWidth - 200) / 2) - 200,
                Convert.ToInt32((screenHeight - 200) / 2),
                800,
                400);
            
            BarcodeScanOptions options = new()
            {
                Formats = BarcodeFormat.All,
                RecognizeMultiple = true,
                IsHighlightingEnabled = true,
                RegionOfInterest = centeredRect,
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
