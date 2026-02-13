using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using System.Diagnostics;

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
            Barcode = (await BarcodeScanner.Default.ScanAsync(new BarcodeScanOptions() { Formats = BarcodeFormat.All }).ConfigureAwait(false)).RawValue;
        }
        catch (BarcodeScanException exception)
        {
            Debug.WriteLine(exception);

            Barcode = "Something went wrong.";
        }
    }
}
