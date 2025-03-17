//
//  DataScannerViewController.swift
//  PluginScanner
//
//  Created by Luca Civale on 15.03.25.
//

import Foundation
import Vision
import VisionKit

typealias VNDataScannerViewController = VisionKit.DataScannerViewController

@objc(DataScannerViewController)
public class DataScannerViewController : NSObject, VisionKit.DataScannerViewControllerDelegate
{
    private var dataScannerViewController: VNDataScannerViewController
    private var dataScannerViewControllerDelegate : DataScannerViewControllerDelegate?
    
    @objc
    @MainActor
    public init(recognizedDataTypes: [RecognizedDataType], qualityLevel: QualityLevel = QualityLevel.balanced, recognizesMultipleItems: Bool = false, isHighFrameRateTrackingEnabled: Bool = true, isPinchToZoomEnabled: Bool = true, isGuidanceEnabled: Bool = true, isHighlightingEnabled: Bool = false)
    {
        var vnRecognizedDataTypes : Set<VNDataScannerViewController.RecognizedDataType> = Set()
        
        for recognizedDataType in recognizedDataTypes
        {
            vnRecognizedDataTypes.insert(recognizedDataType.ToVNRecognizedDataType())
        }

        dataScannerViewController = VNDataScannerViewController.init(recognizedDataTypes: vnRecognizedDataTypes, qualityLevel: qualityLevel.ToVNQualityLevel(), recognizesMultipleItems: recognizesMultipleItems, isHighFrameRateTrackingEnabled: isHighFrameRateTrackingEnabled, isPinchToZoomEnabled: isPinchToZoomEnabled, isGuidanceEnabled: isGuidanceEnabled, isHighlightingEnabled: isHighlightingEnabled)

        super.init()
        
        dataScannerViewController.delegate = self
    }

    @objc
    @MainActor
    public static var isSupported : Bool
    {
        get
        {
            VNDataScannerViewController.isSupported
        }
    }
    
    @objc
    @MainActor
    public static var isAvailable : Bool
    {
        get
        {
            VNDataScannerViewController.isAvailable
        }
    }
    
    @objc
    @MainActor
    public static var supportedTextRecognitionLanguages : [String]
    {
        get
        {
            VNDataScannerViewController.supportedTextRecognitionLanguages
        }
    }
    
    @objc
    public static var scanningUnavailable : [String]
    {
        get
        {
            [VNDataScannerViewController.ScanningUnavailable.cameraRestricted.localizedDescription, VNDataScannerViewController.ScanningUnavailable.unsupported.localizedDescription];
        }
    }
    
    @objc
    public var ViewController : UIViewController
    {
        get
        {
            dataScannerViewController
        }
    }
    
    @objc
    @MainActor
    public var Delegate: DataScannerViewControllerDelegate?
    {
        get
        {
            dataScannerViewControllerDelegate
        }
        set
        {
            dataScannerViewControllerDelegate = newValue
        }
    }

    @objc
    @MainActor
    public var regionOfInterest : CGRect
    {
        get
        {
            dataScannerViewController.regionOfInterest ?? CGRect.zero
        }
        set
        {
            dataScannerViewController.regionOfInterest = newValue
        }
    }

    @objc
    @MainActor
    public var zoomFactor : Double
    {
        get
        {
            dataScannerViewController.zoomFactor
        }
        set
        {
            dataScannerViewController.zoomFactor = newValue
        }
    }

    @objc
    @MainActor
    public var minZoomFactor : Double
    {
        get
        {
            dataScannerViewController.minZoomFactor
        }
    }
    
    @objc
    @MainActor
    public var maxZoomFactor : Double
    {
        get
        {
            dataScannerViewController.maxZoomFactor
        }
    }
    
    @objc
    @MainActor
    public var isScanning : Bool
    {
        get
        {
            dataScannerViewController.isScanning
        }
    }
    
    @objc
    @MainActor
    public var qualityLevel : QualityLevel
    {
        get
        {
            switch dataScannerViewController.qualityLevel
            {
                case VNDataScannerViewController.QualityLevel.accurate:
                    return QualityLevel.accurate
                case VNDataScannerViewController.QualityLevel.balanced:
                    return QualityLevel.balanced
                case VNDataScannerViewController.QualityLevel.fast:
                    return QualityLevel.fast
                @unknown default:
                    return QualityLevel.accurate
            }
        }
    }

    @objc
    @MainActor
    public var recognizesMultipleItems : Bool
    {
        get
        {
            dataScannerViewController.recognizesMultipleItems
        }
    }
    
    @objc
    @MainActor
    public var isHighFrameRateTrackingEnabled : Bool
    {
        get
        {
            dataScannerViewController.isHighFrameRateTrackingEnabled
        }
    }
    
    @objc
    @MainActor
    public var isPinchToZoomEnabled : Bool
    {
        get
        {
            dataScannerViewController.isPinchToZoomEnabled
        }
    }
    
    @objc
    @MainActor
    public var isGuidanceEnabled : Bool
    {
        get
        {
            dataScannerViewController.isGuidanceEnabled
        }
    }
    
    @objc
    @MainActor
    public var isHighlightingEnabled : Bool
    {
        get
        {
            dataScannerViewController.isHighlightingEnabled
        }
    }
    
    @objc
    @MainActor
    public var overlayContainerView : UIView
    {
        get
        {
            dataScannerViewController.overlayContainerView
        }
    }
    
    
    @objc(capturePhoto:)
    public func capturePhoto() async throws -> UIImage?
    {
        do
        {
            return try await dataScannerViewController.capturePhoto();
        }
        catch
        {
            throw error
        }
    }
    
    @objc(StartScanning:)
    @MainActor
    public func startScanning() throws
    {
        do
        {
            try dataScannerViewController.startScanning()
        }
        catch
        {
            throw error;
        }
    }
    
    
    @objc
    @MainActor
    public func stopScanning()
    {
        dataScannerViewController.stopScanning()
    }
    
    @MainActor
    public func dataScannerDidZoom(_ dataScanner: VisionKit.DataScannerViewController)
    {
        dataScannerViewControllerDelegate?.dataScannerDidZoom(self)
    }

    public func dataScanner(_ dataScanner: VisionKit.DataScannerViewController, didTapOn item: VisionKit.RecognizedItem)
    {
        dataScannerViewControllerDelegate?.dataScanner(self, didTapOn: RecognizedItem.fromVNRecognizedItem(recognizedItem: item))
    }

    public func dataScanner(_ dataScanner: VisionKit.DataScannerViewController, didAdd addedItems: [VisionKit.RecognizedItem], allItems: [VisionKit.RecognizedItem])
    {
        dataScannerViewControllerDelegate?.dataScanner(
            self,
            didAdd: addedItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            },
            allItems: allItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            })
    }

    public func dataScanner(_ dataScanner: VisionKit.DataScannerViewController, didUpdate updatedItems: [VisionKit.RecognizedItem], allItems: [VisionKit.RecognizedItem])
    {
        dataScannerViewControllerDelegate?.dataScanner(
            self,
            didAdd: updatedItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            },
            allItems: allItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            })
    }

    public func dataScanner(_ dataScanner: VisionKit.DataScannerViewController, didRemove removedItems: [VisionKit.RecognizedItem], allItems: [VisionKit.RecognizedItem])
    {
        dataScannerViewControllerDelegate?.dataScanner(
            self,
            didAdd: removedItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            },
            allItems: allItems.map
            {
                (item) -> RecognizedItem in RecognizedItem.fromVNRecognizedItem(recognizedItem: item)
            })
    }

    public func dataScanner(_ dataScanner: VisionKit.DataScannerViewController, becameUnavailableWithError error: VisionKit.DataScannerViewController.ScanningUnavailable)
    {
        dataScannerViewControllerDelegate?.dataScanner(self, becameUnavailableWithError: ScanningUnavailable.FromVNScanningUnavailable(scanningUnavailable: error))
    }
}
