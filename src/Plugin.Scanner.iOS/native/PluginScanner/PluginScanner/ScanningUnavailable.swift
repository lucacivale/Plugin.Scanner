//
//  ScanningUnavailable.swift
//  PluginScanner
//
//  Created by Luca Civale on 17.03.25.
//

import Foundation
import VisionKit

@objc
public enum ScanningUnavailable : Int {
    
    case unsupported
    
    case cameraRestricted
    
    public static func FromVNScanningUnavailable(scanningUnavailable : VisionKit.DataScannerViewController.ScanningUnavailable) -> ScanningUnavailable
    {
        switch scanningUnavailable
        {
            case VisionKit.DataScannerViewController.ScanningUnavailable.cameraRestricted:
                ScanningUnavailable.cameraRestricted
            case VisionKit.DataScannerViewController.ScanningUnavailable.unsupported:
                ScanningUnavailable.unsupported
            @unknown default:
                ScanningUnavailable.unsupported
        }
    }
}
