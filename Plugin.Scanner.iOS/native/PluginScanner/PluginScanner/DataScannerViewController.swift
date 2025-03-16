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
public class DataScannerViewController : NSObject
{
    private var dataScannerViewController: VNDataScannerViewController
    
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
    }
    
    @objc(ViewController)
    public func ViewController() -> UIViewController
    {
        return dataScannerViewController;
    }

    @objc
    @MainActor
    public static func isSupported() -> Bool
    {
        return VNDataScannerViewController.isSupported
    }
    
    @objc
    @MainActor
    public static func isAvailable() -> Bool
    {
        return VNDataScannerViewController.isAvailable
    }
    
    @objc
    @MainActor
    public static func supportedTextRecognitionLanguages() -> [String]
    {
        return VNDataScannerViewController.supportedTextRecognitionLanguages
    }
    
    @objc
    public static func scanningUnavailable() -> NSMutableSet
    {
        let reasons = NSMutableSet.init();
        reasons.add(VNDataScannerViewController.ScanningUnavailable.cameraRestricted);
        reasons.add(VNDataScannerViewController.ScanningUnavailable.unsupported);

        return reasons;
    }
    
    @objc(startScanning:)
    @MainActor public func startScan() throws
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
    /*private var scannerCallback: ((_ codes: [String]) -> Void)? = nil
    private var scannerUpdateCallback: ((_ codes: [String]) -> Void)? = nil

    @objc
    @MainActor override init()
    {
        let supportedBarcodes = VNDetectBarcodesRequest.init().symbologies
        let viewController = DataScannerViewController.init(recognizedDataTypes:[.barcode(symbologies:supportedBarcodes)])
        self.viewController = viewController

        super.init()

        self.viewController.delegate = self
    }

    @objc
    public func getViewController() -> UIViewController
    {
        return self.viewController
    }

    private func handleBarcodeCallback(barcodes: [RecognizedItem], callback: ((_ codes: [String]) -> Void)?)
    {
        if(callback != nil)
        {
            let scannedBarcodes: [String] = barcodes.map
            { (item) -> String in
                switch(item)
                {
                case .barcode(let barcode):
                        return barcode.payloadStringValue ?? "<barcode not read>"
                    default:
                        return "<not a barcode>"
                }
            }
            callback!(scannedBarcodes)
        }
    }

    public func dataScanner(_ dataScanner: DataScannerViewController, didAdd addedItems: [RecognizedItem], allItems: [RecognizedItem])
    {
        handleBarcodeCallback(barcodes: addedItems, callback: self.scannerCallback)
    }

    public func dataScanner(_ dataScanner: DataScannerViewController, didUpdate updatedItems: [RecognizedItem], allItems: [RecognizedItem])
    {
        handleBarcodeCallback(barcodes: updatedItems, callback: self.scannerUpdateCallback)
    }

    @objc
    @MainActor public func setScanCallback(callback: @escaping ([String]) -> Void)
    {
        self.scannerCallback = callback
    }

    @objc
    @MainActor public func setScanUpdatedCallback(callback: @escaping ([String]) -> Void)
    {
        self.scannerUpdateCallback = callback
    }

    @objc
    @MainActor public func startScan() throws
    {
        do
        {
            try self.viewController.startScanning()
        }
        catch
        {
            throw error;
        }
    }

    @objc
    @MainActor public func stopScan()
    {
        self.viewController.stopScanning()
    }*/
}

public class Box<T> {
    let unbox: T
    init(_ value: T) {
        self.unbox = value
    } }
