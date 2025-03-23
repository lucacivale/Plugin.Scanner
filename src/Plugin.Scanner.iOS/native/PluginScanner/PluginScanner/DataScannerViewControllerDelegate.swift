//
//  DataScannerViewControllerDelegate.swift
//  PluginScanner
//
//  Created by Luca Civale on 17.03.25.
//

import Foundation

@objc
@MainActor
public protocol DataScannerViewControllerDelegate : AnyObject {
    
    @objc
    @MainActor
    func dataScannerDidZoom(_ dataScanner: DataScannerViewController)

    @objc(didTapOn::)
    @MainActor
    func dataScanner(_ dataScanner: DataScannerViewController, didTapOn item: RecognizedItem)

    @objc(didAdd:::)
    @MainActor
    func dataScanner(_ dataScanner: DataScannerViewController, didAdd addedItems: [RecognizedItem], allItems: [RecognizedItem])

    @objc(didUpdate:::)
    @MainActor
    func dataScanner(_ dataScanner: DataScannerViewController, didUpdate updatedItems: [RecognizedItem], allItems: [RecognizedItem])

    @objc(didRemove:::)
    @MainActor
    func dataScanner(_ dataScanner: DataScannerViewController, didRemove removedItems: [RecognizedItem], allItems: [RecognizedItem])

    @objc(becameUnavailableWithError::)
    @MainActor
    func dataScanner(_ dataScanner: DataScannerViewController, becameUnavailableWithError error: ScanningUnavailable)
}

extension DataScannerViewControllerDelegate {
    
    @MainActor
    public func dataScannerDidZoom(_ dataScanner: DataScannerViewController)
    {
        
    }
    
    @MainActor
    public func dataScanner(_ dataScanner: DataScannerViewController, didTapOn item: RecognizedItem)
    {
        
    }
    
    @MainActor
    public func dataScanner(_ dataScanner: DataScannerViewController, didAdd addedItems: [RecognizedItem], allItems: [RecognizedItem])
    {
        
    }

    @MainActor
    public func dataScanner(_ dataScanner: DataScannerViewController, didUpdate updatedItems: [RecognizedItem], allItems: [RecognizedItem])
    {
        
    }

    @MainActor
    public func dataScanner(_ dataScanner: DataScannerViewController, didRemove removedItems: [RecognizedItem], allItems: [RecognizedItem])
    {
        
    }

    @MainActor
    public func dataScanner(_ dataScanner: DataScannerViewController, becameUnavailableWithError error: ScanningUnavailable)
    {
        
    }
}
