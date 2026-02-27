using AndroidX.Camera.View;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Extensions;
using Plugin.Scanner.Views.Android;

namespace Plugin.Scanner.Overlays.Text;

internal sealed partial class TextScannerOverlay : ScannerOverlay
{
    public override bool OnTouch(View? v, MotionEvent? e)
    {
        if (e?.Action == MotionEventActions.Down
            && RecognizedItems?.FirstOrDefault(x => x.Bounds.ContainsWithTolerance((int)e.GetX(), (int)e.GetY(), 30)) is RecognizedItem item)
        {
            RecognizedItemButton itemButton = Root?.FindViewById<RecognizedItemButton>(_Microsoft.Android.Resource.Designer.Resource.Id.recognizedItemButton) ?? throw new ViewNotFoundException(nameof(RecognizedItemButton));
            itemButton.RecognizedItem = item;
            itemButton.Visibility = ViewStates.Visible;
        }

        return false;
    }

    protected override void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        base.OnDetected(sender, e);

        PreviewView previewView = Dialog?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        if (Dialog?.IsHighlightingEnabled == true
            && Dialog?.RecognizeMultiple == true)
        {
            Color baseColor = Color.Yellow;

            for (int i = 0; i < e.Count; i++)
            {
                float hueOffset = 360f / e.Count * i;

                previewView.Overlay?.Add(new TextBlockHighlight(e[i].Bounds.ToRect(), baseColor, hueOffset));
            }
        }

        previewView.Invalidate();
    }
}
