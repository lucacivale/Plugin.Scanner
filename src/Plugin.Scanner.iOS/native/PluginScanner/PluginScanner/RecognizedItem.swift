//
//  RecognizedItem.swift
//  PluginScanner
//
//  Created by Luca Civale on 16.03.25.
//

import Foundation
import VisionKit

@objc
public class RecognizedItem : NSObject
{
    @objc
    public let id : UUID
    
    @objc
    public let bounds : Bounds
    
    @objc
    public let value : String
    
    public static func fromVNRecognizedItem(recognizedItem: VisionKit.RecognizedItem) -> RecognizedItem
    {
        var text : String
        
        switch recognizedItem
        {
            case .barcode(let barcodeItem):
                text = barcodeItem.payloadStringValue ?? "Could not read barcode"
            case .text(let textItem):
                text = textItem.transcript
            @unknown default:
                text = "Unkown"
        }
        
        return RecognizedItem.init(id: recognizedItem.id, bounds: Bounds.fromRCIBounds(bounds: recognizedItem.bounds), value: text)
    }

    @objc
    public init(id: UUID, bounds: Bounds, value: String) {
        self.id = id
        self.bounds = bounds
        self.value = value
    }
}
