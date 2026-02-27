using AndroidX.Camera.View;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Extensions;
using Plugin.Scanner.Views.Android;

namespace Plugin.Scanner.Overlays.Barcode;

internal sealed class BarcodeScannerOverlay : ScannerOverlay
{
    private RecognizedItem? _selectedRecognizedItem;

    public override bool OnTouch(View? v, MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Down
            && RecognizedItems?.FirstOrDefault(x => x.Bounds.ContainsWithTolerance((int)e.GetX(), (int)e.GetY(), 30)) is RecognizedItem item)
        {
            _selectedRecognizedItem = item;
        }

        return false;
    }

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
                    previewView.Overlay?.Add(new BarcodeHighlight(recognizedItem));
                }
            }
            else
            {
                foreach (RecognizedItem item in e)
                {
                    previewView.Overlay?.Add(new BarcodeHighlight(item));
                }
            }
        }

        previewView.Invalidate();
    }
}