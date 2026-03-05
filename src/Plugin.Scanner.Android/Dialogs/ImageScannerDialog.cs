using Plugin.Scanner.Android.DataDetectors;
using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Core;

namespace Plugin.Scanner.Android.Dialogs;

internal sealed class ImageScannerDialog : DataScannerDialog
{
    private readonly Bitmap? _imageBitmap;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageScannerDialog"/> class.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="detector">The data detector to use for recognition.</param>
    /// <param name="cameraController">The camera controller for managing camera operations.</param>
    /// <param name="regionOfInterest">Optional region of interest to limit scanning area.</param>
    /// <param name="overlay">Optional overlay to display on the scanner view.</param>
    /// <param name="recognizeMultiple">Whether to recognize multiple items.</param>
    /// <param name="isHighlightingEnabled">Whether to highlight detected items.</param>
    public ImageScannerDialog(
        Activity context,
        Bitmap imageBitmap,
        IDataDetector detector,
        IRegionOfInterest? regionOfInterest,
        IOverlay? overlay,
        bool recognizeMultiple,
        bool isHighlightingEnabled)
        : base(context, detector, regionOfInterest, overlay, recognizeMultiple, isHighlightingEnabled)
    {
        _imageBitmap = imageBitmap;
    }

    protected override void Cleanup()
    {
        base.Cleanup();

        ImageView imageView = FindViewById<ImageView>(_Microsoft.Android.Resource.Designer.Resource.Id.imageView) ?? throw new ViewNotFoundException(nameof(ImageView));
        imageView.SetImageBitmap(null);
    }

    protected override void SetContentView()
    {
        SetContentView(_Microsoft.Android.Resource.Designer.Resource.Layout.ImageScanner);

        ImageView imageView = FindViewById<ImageView>(_Microsoft.Android.Resource.Designer.Resource.Id.imageView) ?? throw new ViewNotFoundException(nameof(ImageView));
        imageView.SetImageBitmap(_imageBitmap);

        base.SetContentView();
    }
}
