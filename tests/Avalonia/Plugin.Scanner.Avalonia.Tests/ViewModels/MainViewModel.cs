using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using System.Diagnostics;
using Plugin.Scanner.Core.Extensions;
using Plugin.Scanner.Models;
using Plugin.Scanner.Options;
using Plugin.Scanner.Overlays.Barcode;

namespace Plugin.Scanner.Avalonia.Tests.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _barcode = string.Empty;

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
                Overlay = new DefaultBarcodeScannerOverlay(),
            };

            Barcode = (await BarcodeScanner.Default.ScanAsync(options).ConfigureAwait(false)).Value;
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }
}
