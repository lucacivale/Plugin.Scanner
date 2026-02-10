namespace Plugin.Scanner.Android.Exceptions;

/// <summary>
/// Exception thrown when ML Kit analyzer returns a result that is not a barcode object.
/// </summary>
public sealed class MlKitAnalyzerResultNotBarcodeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MlKitAnalyzerResultNotBarcodeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MlKitAnalyzerResultNotBarcodeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MlKitAnalyzerResultNotBarcodeException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MlKitAnalyzerResultNotBarcodeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MlKitAnalyzerResultNotBarcodeException"/> class.
    /// </summary>
    public MlKitAnalyzerResultNotBarcodeException()
    {
    }
}