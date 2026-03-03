using System.Diagnostics;
using Plugin.Scanner.Core;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.Core.Extensions;
using Plugin.Scanner.Core.Models.Enums;
using Plugin.Scanner.Core.Scanners;
using Plugin.Scanner.Options;

namespace Plugin.Scanner.Uno.Tests.Presentation;

public partial record MainModel
{
    private readonly IBarcodeScanner _barcodeScanner;
    private readonly ITextScanner _textScanner;
    private readonly IDocumentScanner _documentScanner;

    public MainModel(IBarcodeScanner barcodeScanner, ITextScanner textScanner,  IDocumentScanner documentScanner)
    {
        _barcodeScanner = barcodeScanner;
        _textScanner = textScanner;
        _documentScanner = documentScanner;
    }

    public IState<string> Barcode => State<string>.Value(this, () => string.Empty);

    public IState<string> Text => State<string>.Value(this, () => string.Empty);

    public IState<string> ScannedDocuments => State<string>.Value(this, () => string.Empty);
    
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

            string barcode = (await _barcodeScanner.ScanAsync(options).ConfigureAwait(false)).Value;

            await Barcode.SetAsync(barcode);
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            await Barcode.SetAsync("Something went wrong.");
        }
    }

    public async Task ScanText()
    {
        try
        {
            TextScanOptions options = new()
            {
                IsHighlightingEnabled = true,
                RegionOfInterest = new CenteredRegionOfInterest(250, 200),
            };

            string text = (await _textScanner.ScanAsync(options).ConfigureAwait(false)).Value;

            await Text.SetAsync(text);
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            await Text.SetAsync("Something went wrong.");
        }
    }

    public async Task ScanDocuments()
    {
        try
        {
            IDocument document = await _documentScanner.ScanAsync().ConfigureAwait(false);

            await ScannedDocuments.SetAsync($"You scanned a document with {document.Pages.Count} pages").ConfigureAwait(false);
        }
        catch (ScanException exception)
        {
            Debug.WriteLine(exception);

            await ScannedDocuments.SetAsync("Something went wrong.");
        }
    }
}
