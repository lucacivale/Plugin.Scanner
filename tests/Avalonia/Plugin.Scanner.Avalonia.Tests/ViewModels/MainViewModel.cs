using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Extensions;
using Plugin.Scanner.Models;
using Plugin.Scanner.Options;
using System.Diagnostics;

namespace Plugin.Scanner.Avalonia.Tests.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _barcode = string.Empty;
    
    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private string _scannedDocuments = string.Empty;

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

            Barcode = (await BarcodeScanner.Default.ScanAsync(options).ConfigureAwait(false)).Value;
        }
        catch (ScanException exception)
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

            Text = (await TextScanner.Default.ScanAsync(options).ConfigureAwait(false)).Value;
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }

    [RelayCommand]
    public async Task ScanDocument()
    {
        try
        {
            var document = await DocumentScanner.Default.ScanAsync().ConfigureAwait(false);

            ScannedDocuments = $"You scanned {document.Count} documents";
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);
        }
    }
}
