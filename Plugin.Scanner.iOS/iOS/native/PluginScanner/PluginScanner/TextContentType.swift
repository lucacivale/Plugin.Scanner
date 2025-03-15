//
//  TextContentType.swift
//  PluginScanner
//
//  Created by Luca Civale on 15.03.25.
//

import Foundation
import VisionKit

@objc
public enum TextContentType : Int
{
    case Default
    
    case dateTimeDuration

    case emailAddress

    case flightNumber

    case fullStreetAddress

    case shipmentTrackingNumber

    case telephoneNumber

    case URL

    @available(iOS 17.0, *)
    case currency
    
    public func MapToVisionTextContentType() -> VisionKit.DataScannerViewController.TextContentType?
    {
        var visionTextContentType : VisionKit.DataScannerViewController.TextContentType? = nil;
        
        switch self
        {
            case TextContentType.Default:
                visionTextContentType = nil
            case TextContentType.URL:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.URL
            case TextContentType.currency:
                if #available(iOS 17.0, *)
                {
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.currency
                }
                else
                {
                    visionTextContentType = nil
                }
            case TextContentType.dateTimeDuration:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.dateTimeDuration
            case TextContentType.emailAddress:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.emailAddress
            case TextContentType.flightNumber:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.flightNumber
            case TextContentType.fullStreetAddress:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.fullStreetAddress
            case TextContentType.shipmentTrackingNumber:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.shipmentTrackingNumber
            case TextContentType.telephoneNumber:
                visionTextContentType = VisionKit.DataScannerViewController.TextContentType.telephoneNumber
        }
        
        return visionTextContentType;
    }
}
