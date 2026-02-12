using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;
using Plugin.Scanner.iOS.Barcode.Views;
using Plugin.Scanner.iOS.Binding;
using Plugin.Scanner.iOS.Exceptions;
using Plugin.Scanner.iOS.Views;

namespace Plugin.Scanner.iOS.Barcode;

/// <summary>
/// A specialized barcode scanner view controller that scans a single barcode and automatically dismisses after scanning.
/// </summary>
internal sealed class SingleBarcodeScannerViewController : BarcodeScannerViewController
{
    private TaskCompletionSource<string>? _scanCompleteTaskSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleBarcodeScannerViewController"/> class.
    /// </summary>
    /// <param name="recognizedDataTypes">An array of <see cref="RecognizedDataType"/> specifying the types of data to recognize during scanning.</param>
    /// <param name="qualityLevel">The <see cref="QualityLevel"/> for scanning. Default is <see cref="QualityLevel.Balanced"/>.</param>
    /// <param name="isHighFrameRateTrackingEnabled">If <c>true</c>, enables high frame rate tracking for better performance. Default is <c>true</c>.</param>
    /// <param name="isPinchToZoomEnabled">If <c>true</c>, allows pinch-to-zoom gesture. Default is <c>true</c>.</param>
    /// <param name="isGuidanceEnabled">If <c>true</c>, displays guidance to the user. Default is <c>true</c>.</param>
    /// <param name="isHighlightingEnabled">If <c>true</c>, highlights recognized items. Default is <c>true</c>.</param>
    public SingleBarcodeScannerViewController(
        RecognizedDataType[] recognizedDataTypes,
        QualityLevel qualityLevel = QualityLevel.Balanced,
        bool isHighFrameRateTrackingEnabled = true,
        bool isPinchToZoomEnabled = true,
        bool isGuidanceEnabled = true,
        bool isHighlightingEnabled = true)
        : base(recognizedDataTypes, qualityLevel, false, isHighFrameRateTrackingEnabled, isPinchToZoomEnabled, isGuidanceEnabled, isHighlightingEnabled)
    {
    }

    /// <summary>
    /// Asynchronously presents the scanner interface and scans for a single barcode.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the scan operation.</param>
    /// <returns>A <see cref="Task{IBarcode}"/> that represents the asynchronous operation. The task result contains the scanned barcode.</returns>
    /// <exception cref="BarcodeScanException">Thrown when the top view controller cannot be found.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled via the <paramref name="cancellationToken"/>.</exception>
    public async Task<IBarcode> ScanAsync(CancellationToken cancellationToken)
    {
        UIViewController topViewController = WindowUtils.GetTopViewController() ?? throw new BarcodeScanException("Failed to find top UIViewController.");

        StartScanning();

        _scanCompleteTaskSource = new();

        Added += OnAdded;
        Removed += OnRemoved;
        BecameUnavailable += OnBecameUnavailable;
        Canceled += OnCanceled;

        await topViewController.PresentViewControllerAsync(this, true).ConfigureAwait(true);

        string barcode = await _scanCompleteTaskSource.Task.WaitAsync(cancellationToken).ConfigureAwait(true);

        Added -= OnAdded;
        Removed -= OnRemoved;
        BecameUnavailable -= OnBecameUnavailable;
        Canceled -= OnCanceled;

        return new Core.Barcode.Barcode(barcode);
    }

    /// <summary>
    /// Handles the event when a barcode item is added to the scanner view.
    /// Creates and displays a tappable button for the scanned barcode.
    /// </summary>
    /// <param name="sender">The <see cref="DataScannerViewController"/> that raised the event.</param>
    /// <param name="e">A tuple containing the added items and all currently recognized items.</param>
    /// <exception cref="DataScannerEventSenderInvalidTypeException">Thrown when the sender is not of type <see cref="DataScannerViewController"/>.</exception>
    /// <exception cref="DataScannerViewNullReferenceException">Thrown when the scanner's view is null.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed when barcode is removed or when the scanner is closed.")]
    private void OnAdded(object? sender, (RecognizedItem[] AddedItems, RecognizedItem[] AllItems) e)
    {
        const float buttonWidthAnchorAdd = 30f;
        const float buttonBottomAnchorAdd = 25f;

        if (sender is not DataScannerViewController scanner)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type DataScannerViewController.");
        }

        _ = scanner.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        BarcodeItemButton barcodeItemButton = new(e.AddedItems[0]);
        barcodeItemButton.TouchUpInside += OnBarcodeItemTapped;

        scanner.View.Add(barcodeItemButton);

        NSLayoutConstraint.ActivateConstraints(
        [
            barcodeItemButton.CenterXAnchor.ConstraintEqualTo(scanner.View.CenterXAnchor),
            barcodeItemButton.BottomAnchor.ConstraintEqualTo(scanner.View.BottomAnchor, -(DataScannerBarOverlay.Height + buttonBottomAnchorAdd)),
            barcodeItemButton.WidthAnchor.ConstraintLessThanOrEqualTo(scanner.View.WidthAnchor, constant: -buttonWidthAnchorAdd),
        ]);
    }

    /// <summary>
    /// Handles the event when a barcode item is removed from the scanner view.
    /// Cleans up the associated button UI element.
    /// </summary>
    /// <param name="sender">The <see cref="DataScannerViewController"/> that raised the event.</param>
    /// <param name="e">A tuple containing the removed items and all currently recognized items.</param>
    /// <exception cref="DataScannerEventSenderInvalidTypeException">Thrown when the sender is not of type <see cref="DataScannerViewController"/>.</exception>
    /// <exception cref="DataScannerViewNullReferenceException">Thrown when the scanner's view is null.</exception>
    private void OnRemoved(object? sender, (RecognizedItem[] RemovedItems, RecognizedItem[] AllItems) e)
    {
        if (sender is not DataScannerViewController scanner)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type DataScannerViewController.");
        }

        _ = scanner.View ?? throw new DataScannerViewNullReferenceException("View can not be null here.");

        BarcodeItemButton? barcodeItem = scanner.View.Subviews
            .OfType<BarcodeItemButton>()
            .FirstOrDefault(x => e.RemovedItems[0].Id.Equals(x.Barcode.Id));

        if (barcodeItem is not null)
        {
            CleanupBarcodeItem(barcodeItem);
        }
    }

    /// <summary>
    /// Handles the event when the user taps a barcode item button.
    /// Stops scanning, dismisses the scanner, and completes the scan operation with the selected barcode.
    /// </summary>
    /// <param name="sender">The <see cref="BarcodeItemButton"/> that was tapped.</param>
    /// <param name="e">The event arguments.</param>
    /// <exception cref="DataScannerViewControllerNotFoundException">Thrown when the <see cref="DataScannerViewController"/> cannot be found.</exception>
    /// <exception cref="DataScannerEventSenderInvalidTypeException">Thrown when the sender is not of type <see cref="BarcodeItemButton"/>.</exception>
    private void OnBarcodeItemTapped(object? sender, EventArgs e)
    {
        if (WindowUtils.GetTopViewController() is not DataScannerViewController scanner)
        {
            throw new DataScannerViewControllerNotFoundException("Failed to find DataScannerViewController.");
        }

        if (sender is not BarcodeItemButton barcodeItem)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type BarcodeItemButton.");
        }

        CleanupBarcodeItems(scanner);

        scanner.StopScanning();
        scanner.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(barcodeItem.Barcode.Value));
    }

    /// <summary>
    /// Handles the event when the scanner becomes unavailable due to an error.
    /// Stops scanning, dismisses the scanner, and completes the scan operation with an exception.
    /// </summary>
    /// <param name="sender">The <see cref="DataScannerViewController"/> that raised the event.</param>
    /// <param name="e">The <see cref="DataScannerUnavailableException"/> containing error details.</param>
    /// <exception cref="DataScannerEventSenderInvalidTypeException">Thrown when the sender is not of type <see cref="DataScannerViewController"/>.</exception>
    private void OnBecameUnavailable(object? sender, DataScannerUnavailableException e)
    {
        if (sender is not DataScannerViewController scanner)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type DataScannerViewController.");
        }

        CleanupBarcodeItems(scanner);

        scanner.StopScanning();
        scanner.DismissViewController(true, () => throw e);
    }

    /// <summary>
    /// Handles the event when the user cancels the scan operation.
    /// Stops scanning, dismisses the scanner, and completes the scan operation with an empty result.
    /// </summary>
    /// <param name="sender">The <see cref="DataScannerViewController"/> that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <exception cref="DataScannerEventSenderInvalidTypeException">Thrown when the sender is not of type <see cref="DataScannerViewController"/>.</exception>
    private void OnCanceled(object? sender, EventArgs e)
    {
        if (sender is not DataScannerViewController scanner)
        {
            throw new DataScannerEventSenderInvalidTypeException("sender must be of type DataScannerViewController.");
        }

        CleanupBarcodeItems(scanner);

        scanner.StopScanning();
        scanner.DismissViewController(true, () => _scanCompleteTaskSource?.TrySetResult(string.Empty));
    }

    /// <summary>
    /// Cleans up all barcode item buttons from the scanner view.
    /// </summary>
    /// <param name="scanner">The <see cref="DataScannerViewController"/> containing the barcode item buttons to clean up.</param>
    private void CleanupBarcodeItems(DataScannerViewController scanner)
    {
        IEnumerable<BarcodeItemButton> barcodeItems = scanner.View?.Subviews.OfType<BarcodeItemButton>() ?? [];

        foreach (BarcodeItemButton barcodeItem in barcodeItems)
        {
            CleanupBarcodeItem(barcodeItem);
        }
    }

    /// <summary>
    /// Removes and disposes a single barcode item button.
    /// </summary>
    /// <param name="barcodeItemButton">The <see cref="BarcodeItemButton"/> to clean up.</param>
    private void CleanupBarcodeItem(BarcodeItemButton barcodeItemButton)
    {
        barcodeItemButton.TouchUpInside -= OnBarcodeItemTapped;
        barcodeItemButton.RemoveFromSuperview();
        barcodeItemButton.Dispose();
    }
}