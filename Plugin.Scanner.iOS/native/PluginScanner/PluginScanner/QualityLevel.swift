//
//  QualityLevel.swift
//  PluginScanner
//
//  Created by Luca Civale on 15.03.25.
//

import Foundation
import VisionKit

@objc
public enum QualityLevel : Int
{
    case balanced

    case fast

    case accurate
    
    public func ToVNQualityLevel() -> VisionKit.DataScannerViewController.QualityLevel
    {
        switch self
        {
            case QualityLevel.accurate:
                return VisionKit.DataScannerViewController.QualityLevel.accurate
            case QualityLevel.balanced:
                return VisionKit.DataScannerViewController.QualityLevel.balanced
            case QualityLevel.fast:
                return VisionKit.DataScannerViewController.QualityLevel.fast
        }
    }
}
