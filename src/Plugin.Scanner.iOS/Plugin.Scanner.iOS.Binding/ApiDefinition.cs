using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Plugin.Scanner.iOS.Binding
{
    // @interface Bounds : NSObject
    [BaseType (typeof(NSObject), Name = "_TtC13PluginScanner6Bounds")]
    [DisableDefaultCtor]
    interface Bounds
    {
        // @property (readonly, nonatomic) CGPoint topLeft;
        [Export ("topLeft")]
        CGPoint TopLeft { get; }

        // @property (readonly, nonatomic) CGPoint topRight;
        [Export ("topRight")]
        CGPoint TopRight { get; }

        // @property (readonly, nonatomic) CGPoint bottomRight;
        [Export ("bottomRight")]
        CGPoint BottomRight { get; }

        // @property (readonly, nonatomic) CGPoint bottomLeft;
        [Export ("bottomLeft")]
        CGPoint BottomLeft { get; }

        // -(instancetype _Nonnull)initWithTopLeft:(CGPoint)topLeft topRight:(CGPoint)topRight bottomRight:(CGPoint)bottomRight bottomLeft:(CGPoint)bottomLeft __attribute__((objc_designated_initializer));
        [Export ("initWithTopLeft:topRight:bottomRight:bottomLeft:")]
        [DesignatedInitializer]
        NativeHandle Constructor (CGPoint topLeft, CGPoint topRight, CGPoint bottomRight, CGPoint bottomLeft);
    }

	// @interface DataScannerViewController : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DataScannerViewController
	{
		// -(instancetype _Nonnull)initWithRecognizedDataTypes:(NSArray<RecognizedDataType *> * _Nonnull)recognizedDataTypes qualityLevel:(enum QualityLevel)qualityLevel recognizesMultipleItems:(BOOL)recognizesMultipleItems isHighFrameRateTrackingEnabled:(BOOL)isHighFrameRateTrackingEnabled isPinchToZoomEnabled:(BOOL)isPinchToZoomEnabled isGuidanceEnabled:(BOOL)isGuidanceEnabled isHighlightingEnabled:(BOOL)isHighlightingEnabled __attribute__((objc_designated_initializer));
		[Export ("initWithRecognizedDataTypes:qualityLevel:recognizesMultipleItems:isHighFrameRateTrackingEnabled:isPinchToZoomEnabled:isGuidanceEnabled:isHighlightingEnabled:")]
		[DesignatedInitializer]
		NativeHandle Constructor (RecognizedDataType[] recognizedDataTypes, QualityLevel qualityLevel, bool recognizesMultipleItems, bool isHighFrameRateTrackingEnabled, bool isPinchToZoomEnabled, bool isGuidanceEnabled, bool isHighlightingEnabled);

		// @property (readonly, nonatomic, class) BOOL isSupported;
		[Static]
		[Export ("isSupported")]
		bool IsSupported { get; }

		// @property (readonly, nonatomic, class) BOOL isAvailable;
		[Static]
		[Export ("isAvailable")]
		bool IsAvailable { get; }

		// @property (readonly, copy, nonatomic, class) NSArray<NSString *> * _Nonnull supportedTextRecognitionLanguages;
		[Static]
		[Export ("supportedTextRecognitionLanguages", ArgumentSemantic.Copy)]
		string[] SupportedTextRecognitionLanguages { get; }

		// @property (readonly, copy, nonatomic, class) NSArray<NSString *> * _Nonnull scanningUnavailable;
		[Static]
		[Export ("scanningUnavailable", ArgumentSemantic.Copy)]
		string[] ScanningUnavailable { get; }

		// @property (readonly, nonatomic, strong) UIViewController * _Nonnull ViewController;
		[Export ("ViewController", ArgumentSemantic.Strong)]
		UIViewController ViewController { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		DataScannerViewControllerDelegate Delegate { get; set; }

		// @property (nonatomic, strong) id<DataScannerViewControllerDelegate> _Nullable Delegate;
		[NullAllowed, Export ("Delegate", ArgumentSemantic.Strong)]
		NSObject WeakDelegate { get; set; }

		// @property (nonatomic) CGRect regionOfInterest;
		[Export ("regionOfInterest", ArgumentSemantic.Assign)]
		CGRect RegionOfInterest { get; set; }

		// @property (nonatomic) double zoomFactor;
		[Export ("zoomFactor")]
		double ZoomFactor { get; set; }

		// @property (readonly, nonatomic) double minZoomFactor;
		[Export ("minZoomFactor")]
		double MinZoomFactor { get; }

		// @property (readonly, nonatomic) double maxZoomFactor;
		[Export ("maxZoomFactor")]
		double MaxZoomFactor { get; }

		// @property (readonly, nonatomic) BOOL isScanning;
		[Export ("isScanning")]
		bool IsScanning { get; }

		// @property (readonly, nonatomic) enum QualityLevel qualityLevel;
		[Export ("qualityLevel")]
		QualityLevel QualityLevel { get; }

		// @property (readonly, nonatomic) BOOL recognizesMultipleItems;
		[Export ("recognizesMultipleItems")]
		bool RecognizesMultipleItems { get; }

		// @property (readonly, nonatomic) BOOL isHighFrameRateTrackingEnabled;
		[Export ("isHighFrameRateTrackingEnabled")]
		bool IsHighFrameRateTrackingEnabled { get; }

		// @property (readonly, nonatomic) BOOL isPinchToZoomEnabled;
		[Export ("isPinchToZoomEnabled")]
		bool IsPinchToZoomEnabled { get; }

		// @property (readonly, nonatomic) BOOL isGuidanceEnabled;
		[Export ("isGuidanceEnabled")]
		bool IsGuidanceEnabled { get; }

		// @property (readonly, nonatomic) BOOL isHighlightingEnabled;
		[Export ("isHighlightingEnabled")]
		bool IsHighlightingEnabled { get; }

		// @property (readonly, nonatomic, strong) UIView * _Nonnull overlayContainerView;
		[Export ("overlayContainerView", ArgumentSemantic.Strong)]
		UIView OverlayContainerView { get; }

        // -(void)recognizedItems:(void (^ _Nonnull)(NSArray<RecognizedItem *> * _Nonnull))completionHandler;
        [Export ("recognizedItems:")]
        void RecognizedItems (Action<NSArray<RecognizedItem>> completionHandler);

		// -(void)capturePhoto:(void (^ _Nonnull)(UIImage * _Nullable, NSError * _Nullable))completionHandler;
		[Export ("capturePhoto:")]
		void CapturePhoto (Action<UIImage, NSError> completionHandler);

		// -(BOOL)StartScanning:(NSError * _Nullable * _Nullable)error;
		[Export ("StartScanning:")]
		bool StartScanning ([NullAllowed] out NSError error);

		// -(void)stopScanning;
		[Export ("stopScanning")]
		void StopScanning ();
	}

	// @protocol DataScannerViewControllerDelegate
	[Protocol (Name = "_TtP13PluginScanner33DataScannerViewControllerDelegate_")]
    [BaseType(typeof(NSObject))]
    [Model]
	interface DataScannerViewControllerDelegate
	{
		// @required -(void)dataScannerDidZoom:(DataScannerViewController * _Nonnull)dataScanner;
		[Abstract]
		[Export ("dataScannerDidZoom:")]
		void DataScannerDidZoom (DataScannerViewController dataScanner);

		// @required -(void)didTapOn:(DataScannerViewController * _Nonnull)dataScanner :(RecognizedItem * _Nonnull)item;
		[Abstract]
		[Export ("didTapOn::")]
		void DidTapOn (DataScannerViewController dataScanner, RecognizedItem item);

		// @required -(void)didAdd:(DataScannerViewController * _Nonnull)dataScanner :(NSArray<RecognizedItem *> * _Nonnull)addedItems :(NSArray<RecognizedItem *> * _Nonnull)allItems;
		[Abstract]
		[Export ("didAdd:::")]
		void DidAdd (DataScannerViewController dataScanner, RecognizedItem[] addedItems, RecognizedItem[] allItems);

		// @required -(void)didUpdate:(DataScannerViewController * _Nonnull)dataScanner :(NSArray<RecognizedItem *> * _Nonnull)updatedItems :(NSArray<RecognizedItem *> * _Nonnull)allItems;
		[Abstract]
		[Export ("didUpdate:::")]
		void DidUpdate (DataScannerViewController dataScanner, RecognizedItem[] updatedItems, RecognizedItem[] allItems);

		// @required -(void)didRemove:(DataScannerViewController * _Nonnull)dataScanner :(NSArray<RecognizedItem *> * _Nonnull)removedItems :(NSArray<RecognizedItem *> * _Nonnull)allItems;
		[Abstract]
		[Export ("didRemove:::")]
		void DidRemove (DataScannerViewController dataScanner, RecognizedItem[] removedItems, RecognizedItem[] allItems);

		// @required -(void)becameUnavailableWithError:(DataScannerViewController * _Nonnull)dataScanner :(enum ScanningUnavailable)error;
		[Abstract]
		[Export ("becameUnavailableWithError::")]
		void BecameUnavailableWithError (DataScannerViewController dataScanner, ScanningUnavailable error);
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

	// @interface RecognizedItem : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC13PluginScanner14RecognizedItem")]
	[DisableDefaultCtor]
	interface RecognizedItem : INativeObject
	{
		// @property (readonly, copy, nonatomic) NSUUID * _Nonnull id;
		[Export ("id", ArgumentSemantic.Copy)]
		NSUuid Id { get; }

		// @property (readonly, nonatomic, strong) Bounds * _Nonnull bounds;
		[Export ("bounds", ArgumentSemantic.Strong)]
		Bounds Bounds { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull value;
		[Export ("value")]
		string Value { get; }

		// -(instancetype _Nonnull)initWithId:(NSUUID * _Nonnull)id bounds:(Bounds * _Nonnull)bounds value:(NSString * _Nonnull)value __attribute__((objc_designated_initializer));
		[Export ("initWithId:bounds:value:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUuid id, Bounds bounds, string value);
	}
}