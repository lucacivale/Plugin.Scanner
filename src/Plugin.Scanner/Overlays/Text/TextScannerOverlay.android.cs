using AndroidX.Camera.View;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.Extensions;
using Plugin.Scanner.Views.Android;

namespace Plugin.Scanner.Overlays.Text;

/// <summary>
/// Provides Android-specific text scanner overlay implementation with touch interaction and visual highlighting.
/// </summary>
internal sealed partial class TextScannerOverlay : ScannerOverlay
{
    private readonly List<TextBlockHighlight> _textBlockHighlights = [];

    /// <summary>
    /// Handles touch events on the scanner view to select text blocks.
    /// </summary>
    /// <param name="v">The view that was touched.</param>
    /// <param name="e">The motion event containing touch data.</param>
    /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Handles the detection of text blocks and updates the overlay with colored highlights for each block.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The list of recognized text items.</param>
    protected override void OnDetected(object? sender, IReadOnlyList<RecognizedItem> e)
    {
        base.OnDetected(sender, e);

        PreviewView previewView = Dialog?.FindViewById<PreviewView>(_Microsoft.Android.Resource.Designer.Resource.Id.previewView) ?? throw new ViewNotFoundException(nameof(PreviewView));
        previewView.Overlay?.Clear();

        _textBlockHighlights.ForEach(x => x.Dispose());
        _textBlockHighlights.Clear();

        if (Dialog?.IsHighlightingEnabled == true
            && Dialog?.RecognizeMultiple == true)
        {
            Color baseColor = Color.Yellow;

            for (int i = 0; i < e.Count; i++)
            {
                float hueOffset = 360f / e.Count * i;

                TextBlockHighlight highlight = new(e[i].Bounds.ToRect(), baseColor, hueOffset);
                _textBlockHighlights.Add(highlight);
                previewView.Overlay?.Add(highlight);
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

        _textBlockHighlights.ForEach(x => x.Dispose());
        _textBlockHighlights.Clear();
    }
}
