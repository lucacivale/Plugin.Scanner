//
//  RecognizedDataType.swift
//  PluginScanner
//
//  Created by Luca Civale on 15.03.25.
//

import Foundation
import Vision
import VisionKit

@objc
public class RecognizedDataType : NSObject
{
    private var languages: [String]?
    private var textContentType : VisionKit.DataScannerViewController.TextContentType?
    
    private var symbologies: [VNBarcodeSymbology]?

    init(languages: [String] = [], textContentType: VisionKit.DataScannerViewController.TextContentType? = nil)
    {
        super.init()
        
        self.languages = languages
        self.textContentType = textContentType
    }
    
    init(symbologies: [VNBarcodeSymbology] = [])
    {
        super.init()
        
        self.symbologies = symbologies
    }

    @objc
    public static func text(languages: [String] = [], textContenttype: TextContentType = TextContentType.Default) -> RecognizedDataType
    {
        return RecognizedDataType.init(languages: languages, textContentType: textContenttype.MapToVisionTextContentType());
    }
    
    @objc
    public static func barcode(symbologies: [VNBarcodeSymbology] = []) -> RecognizedDataType
    {
        return RecognizedDataType.init(symbologies: symbologies);
    }
    
    public func ToVNRecognizedDataType() -> VisionKit.DataScannerViewController.RecognizedDataType
    {
        if (languages != nil)
        {
            return VisionKit.DataScannerViewController.RecognizedDataType.text(languages: languages!, textContentType: textContentType);
        }
        else
        {
            return VisionKit.DataScannerViewController.RecognizedDataType.barcode(symbologies: symbologies!);
        }
    }
}
