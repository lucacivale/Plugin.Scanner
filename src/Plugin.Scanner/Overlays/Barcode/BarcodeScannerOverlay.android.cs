using AndroidX.Camera.View;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Extensions;
using Plugin.Scanner.Views.Android;

namespace Plugin.Scanner.Overlays.Barcode;

/// <summary>
/// Provides Android-specific barcode scanner overlay implementation with touch interaction and visual highlighting.
/// </summary>
internal sealed partial class BarcodeScannerOverlay : ScannerOverlay
{
    private readonly List<BarcodeHighlight> _barcodeHighlights = [];

    private RecognizedItem? _selectedRecognizedItem;

    /// <summary>
    /// Handles touch events on the scanner view to select barcodes.
    /// </summary>
    /// <param name="v">The view that was touched.</param>
    /// <param name="e">The motion event containing touch data.</param>
    /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
    public override bool OnTouch(View? v, MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Down
            && RecognizedItems?.FirstOrDefault(x => x.Bounds.ContainsWithTolerance((int)e.GetX(), (int)e.GetY(), 30)) is RecognizedItem item)
        {
            _selectedRecognizedItem = item;
        }

        return false;
    }

    /// <summary>
    /// Handles the detection of barcodes and updates the overlay UI with highlights and selection button.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The list of recognized barcode items.</param>
    protected override void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        base.OnDetected(sender, e);

        PreviewView previewView = Dialog?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));

        RecognizedItem? recognizedItem;

        if (Dialog?.RecognizeMultiple == false)
        {
            _selectedRecognizedItem ??= e.First();

            recognizedItem = e.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) ?? e.First();
        }
        else
        {
            recognizedItem = e.FirstOrDefault(x => x.Equals(_selectedRecognizedItem)) ?? null;
        }

        RecognizedItemButton itemButton = Root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
        itemButton.RecognizedItem = recognizedItem;
        itemButton.Visibility = recognizedItem is null ? ViewStates.Gone : ViewStates.Visible;

        if (Dialog?.IsHighlightingEnabled == true)
        {
            if (Dialog?.RecognizeMultiple == false)
            {
                if (recognizedItem is not null)
                {
                    BarcodeHighlight highlight = new(recognizedItem);
                    _barcodeHighlights.Add(highlight);
                    previewView.Overlay?.Add(highlight);
                }
            }
            else
            {
                foreach (RecognizedItem item in e)
                {
                    BarcodeHighlight highlight = new(item);
                    _barcodeHighlights.Add(highlight);
                    previewView.Overlay?.Add(highlight);
                }
            }
        }

        previewView.Invalidate();
    }

    /// <summary>
    /// Handles the cleared event when no items are detected and hides the recognition UI.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected override void OnCleared(object? sender, EventArgs e)
    {
        base.OnCleared(sender, e);

        _barcodeHighlights.ForEach(x => x.Dispose());
        _barcodeHighlights.Clear();
    }
}