using Foundation;
using ObjCRuntime;
//using PluginScanner;

namespace Plugin.Scanner.iOS.Binding
{
    /*
	// @interface DataScannerViewController : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DataScannerViewController
	{
		// -(instancetype _Nonnull)initWithRecognizedDataTypes:(NSArray<RecognizedDataType *> * _Nonnull)recognizedDataTypes qualityLevel:(enum QualityLevel)qualityLevel recognizesMultipleItems:(BOOL)recognizesMultipleItems isHighFrameRateTrackingEnabled:(BOOL)isHighFrameRateTrackingEnabled isPinchToZoomEnabled:(BOOL)isPinchToZoomEnabled isGuidanceEnabled:(BOOL)isGuidanceEnabled isHighlightingEnabled:(BOOL)isHighlightingEnabled __attribute__((objc_designated_initializer));
		[Export ("initWithRecognizedDataTypes:qualityLevel:recognizesMultipleItems:isHighFrameRateTrackingEnabled:isPinchToZoomEnabled:isGuidanceEnabled:isHighlightingEnabled:")]
		[DesignatedInitializer]
		NativeHandle Constructor (RecognizedDataType[] recognizedDataTypes, QualityLevel qualityLevel, bool recognizesMultipleItems, bool isHighFrameRateTrackingEnabled, bool isPinchToZoomEnabled, bool isGuidanceEnabled, bool isHighlightingEnabled);

		// +(BOOL)isSupported __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isSupported")]
		[Verify (MethodToProperty)]
		bool IsSupported { get; }

		// +(BOOL)isAvailable __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isAvailable")]
		[Verify (MethodToProperty)]
		bool IsAvailable { get; }

		// +(NSArray<NSString *> * _Nonnull)supportedTextRecognitionLanguages __attribute__((warn_unused_result("")));
		[Static]
		[Export ("supportedTextRecognitionLanguages")]
		[Verify (MethodToProperty)]
		string[] SupportedTextRecognitionLanguages { get; }

		// +(NSMutableSet * _Nonnull)ScanningUnavailable __attribute__((warn_unused_result("")));
		[Static]
		[Export ("ScanningUnavailable")]
		[Verify (MethodToProperty)]
		NSMutableSet ScanningUnavailable { get; }
	}

	// @interface RecognizedDataType : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC13PluginScanner18RecognizedDataType")]
	[DisableDefaultCtor]
	interface RecognizedDataType
	{
		// +(RecognizedDataType * _Nonnull)textWithLanguages:(NSArray<NSString *> * _Nonnull)languages textContenttype:(enum TextContentType)textContenttype __attribute__((warn_unused_result("")));
		[Static]
		[Export ("textWithLanguages:textContenttype:")]
		RecognizedDataType TextWithLanguages (string[] languages, TextContentType textContenttype);

		// +(RecognizedDataType * _Nonnull)barcodeWithSymbologies:(NSArray<VNBarcodeSymbology> * _Nonnull)symbologies __attribute__((warn_unused_result("")));
		[Static]
		[Export ("barcodeWithSymbologies:")]
		RecognizedDataType BarcodeWithSymbologies (string[] symbologies);
	}
    */
	
}
