using Foundation;
using ObjCRuntime;
using UIKit;

namespace Plugin.Scanner.iOS.Binding
{
	// @interface DataScannerViewController : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DataScannerViewController
	{
		// -(instancetype _Nonnull)initWithRecognizedDataTypes:(NSArray<RecognizedDataType *> * _Nonnull)recognizedDataTypes qualityLevel:(enum QualityLevel)qualityLevel recognizesMultipleItems:(BOOL)recognizesMultipleItems isHighFrameRateTrackingEnabled:(BOOL)isHighFrameRateTrackingEnabled isPinchToZoomEnabled:(BOOL)isPinchToZoomEnabled isGuidanceEnabled:(BOOL)isGuidanceEnabled isHighlightingEnabled:(BOOL)isHighlightingEnabled __attribute__((objc_designated_initializer));
		[Export ("initWithRecognizedDataTypes:qualityLevel:recognizesMultipleItems:isHighFrameRateTrackingEnabled:isPinchToZoomEnabled:isGuidanceEnabled:isHighlightingEnabled:")]
		[DesignatedInitializer]
		NativeHandle Constructor (RecognizedDataType[] recognizedDataTypes, QualityLevel qualityLevel, bool recognizesMultipleItems, bool isHighFrameRateTrackingEnabled, bool isPinchToZoomEnabled, bool isGuidanceEnabled, bool isHighlightingEnabled);

        // -(UIViewController * _Nullable)ViewController __attribute__((warn_unused_result("")));
        [NullAllowed, Export ("ViewController")]
        UIViewController ViewController { get; }

		// +(BOOL)isSupported __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isSupported")]
		bool IsSupported { get; }

		// +(BOOL)isAvailable __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isAvailable")]
		bool IsAvailable { get; }

		// +(NSArray<NSString *> * _Nonnull)supportedTextRecognitionLanguages __attribute__((warn_unused_result("")));
		[Static]
		[Export ("supportedTextRecognitionLanguages")]
		string[] SupportedTextRecognitionLanguages { get; }

		// +(NSMutableSet * _Nonnull)scanningUnavailable __attribute__((warn_unused_result("")));
		[Static]
		[Export ("scanningUnavailable")]
		NSMutableSet ScanningUnavailable { get; }
	}

	// @interface RecognizedDataType : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC13PluginScanner18RecognizedDataType")]
	[DisableDefaultCtor]
	interface RecognizedDataType
	{
		// +(RecognizedDataType * _Nonnull)text:(NSArray<NSString *> * _Nonnull)languages :(enum TextContentType)textContenttype __attribute__((warn_unused_result("")));
		[Static]
		[Export ("text::")]
		RecognizedDataType Text (string[] languages, TextContentType textContenttype);

		// +(RecognizedDataType * _Nonnull)barcode:(NSArray<VNBarcodeSymbology> * _Nonnull)symbologies __attribute__((warn_unused_result("")));
		[Static]
		[Export ("barcode:")]
		RecognizedDataType Barcode (string[] symbologies);
	}
}
