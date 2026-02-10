using Plugin.Scanner.Android.Exceptions;
using Plugin.Scanner.Android.Extensions;
using Plugin.Scanner.Core.Barcode;
using Plugin.Scanner.Core.Exceptions;

namespace Plugin.Scanner.Android.Barcode;

/// <summary>
/// Provides Android-specific implementation of the barcode scanner interface using Google ML Kit.
/// </summary>
/// <remarks>
/// <para>
/// This class implements <see cref="IBarcodeScanner"/> for Android devices and uses Google ML Kit's
/// barcode scanning capabilities through a camera dialog interface.
/// </para>
/// <para>
/// The scanner presents a full-screen camera interface with visual barcode highlighting and
/// requires the <c>CAMERA</c> permission to be granted.
/// </para>
/// </remarks>
internal sealed class BarcodeScanner : IBarcodeScanner
{
    private readonly ICurrentActivity _currentActivity;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeScanner"/> class.
    /// </summary>
    /// <param name="currentActivity">The current activity provider for accessing the Android activity context.</param>
    public BarcodeScanner(ICurrentActivity currentActivity)
    {
        _currentActivity = currentActivity;
    }

    /// <summary>
    /// Asynchronously scans for a barcode using the device camera with ML Kit barcode detection.
    /// </summary>
    /// <param name="options">The scan options specifying which barcode formats to recognize.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the scan operation.</param>
    /// <returns>
    /// A task that represents the asynchronous scan operation. The task result contains
    /// the scanned barcode with its decoded value.
    /// </returns>
    /// <exception cref="BarcodeScanException">
    /// Thrown when the scan operation fails due to one of the following reasons:
    /// <list type="bullet">
    /// <item><description>The device has no camera available</description></item>
    /// <item><description>The main executor is not available</description></item>
    /// <item><description>Required UI views cannot be found</description></item>
    /// <item><description>ML Kit analyzer returns an unexpected result type</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation is canceled via the <paramref name="cancellationToken"/>
    /// or by the user dismissing the scanner dialog.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method creates and displays a <see cref="SingleBarcodeScannerDialog"/> that handles
    /// the camera preview, barcode detection, and user interaction.
    /// </para>
    /// <para>
    /// The dialog is automatically disposed after the scan completes or is canceled.
    /// </para>
    /// </remarks>
    public async Task<IBarcode> ScanAsync(IBarcodeScanOptions options, CancellationToken cancellationToken)
    {
        try
        {
            using SingleBarcodeScannerDialog scannerDialog = new(_currentActivity.GetActivity(), options.Formats.ToBarcodeFormats());

            return await scannerDialog.ScanAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception e)
            when (e is MainExecutorNotAvailableException
                      or MlKitAnalyzerResultNotBarcodeException
                      or NoCameraException
                      or ViewNotFoundException)
        {
            throw new BarcodeScanException(e.Message, e);
        }
    }
}