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
    
    init(symbologies: [String] = [])
    {
        super.init()
        
        self.symbologies = [];
        
        for symbology in symbologies {
            switch symbology
            {
                case "Aztec":
                    self.symbologies?.append(.aztec)
                case "Codabar":
                    self.symbologies?.append(.codabar)
                case "Code39":
                    self.symbologies?.append(.code39)
                case "Code39Checksum":
                    self.symbologies?.append(.code39Checksum)
                case "Code39FullASCII":
                    self.symbologies?.append(.code39FullASCII)
                case "Code39FullASCIIChecksum":
                    self.symbologies?.append(.code39FullASCIIChecksum)
                case "Code93":
                    self.symbologies?.append(.code93)
                case "Code93i":
                    self.symbologies?.append(.code93i)
                case "Code128":
                    self.symbologies?.append(.code128)
                case "DataMatrix":
                    self.symbologies?.append(.dataMatrix)
                case "Ean8":
                    self.symbologies?.append(.ean8)
                case "Ean13":
                    self.symbologies?.append(.ean13)
                case "Gs1DataBarExpanded":
                    self.symbologies?.append(.gs1DataBarExpanded)
                case "Gs1DataBarLimited":
                    self.symbologies?.append(.gs1DataBarLimited)
                case "Gs1DataBar":
                    self.symbologies?.append(.gs1DataBar)
                case "I2of5":
                    self.symbologies?.append(.i2of5)
                case "I2of5Checksum":
                    self.symbologies?.append(.i2of5Checksum)
                case "Itf14":
                    self.symbologies?.append(.itf14)
                case "MicroPDF417":
                    self.symbologies?.append(.microPDF417)
                case "MicroQR":
                    self.symbologies?.append(.microQR)
                case "MsiPlessey":
                    if #available(iOS 17.0, *)
                    {
                        self.symbologies?.append(.msiPlessey)
                    }
                case "Pdf417":
                    self.symbologies?.append(.pdf417)
                case "QR":
                    self.symbologies?.append(.qr)
                case "Upce":
                    self.symbologies?.append(.upce)
                default:
                    break
            }
        }
    }

    @objc(text::)
    public static func text(languages: [String] = [], textContenttype: TextContentType = TextContentType.Default) -> RecognizedDataType
    {
        return RecognizedDataType.init(languages: languages, textContentType: textContenttype.MapToVisionTextContentType());
    }
    
    @objc(barcode:)
    public static func barcode(symbologies: [String] = []) -> RecognizedDataType
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
            return VisionKit.DataScannerViewController.RecognizedDataType.barcode(symbologies: symbologies!)
        }
    }
}
