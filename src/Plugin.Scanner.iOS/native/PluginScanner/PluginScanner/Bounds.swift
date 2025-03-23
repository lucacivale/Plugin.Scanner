//
//  Bounds.swift
//  PluginScanner
//
//  Created by Luca Civale on 16.03.25.
//

import Foundation
import VisionKit

@objc
public class Bounds : NSObject
{
    @objc
    public let topLeft: CGPoint

    @objc
    public let topRight: CGPoint

    @objc
    public let bottomRight: CGPoint

    @objc
    public let bottomLeft: CGPoint
    
    public static func fromRCIBounds(bounds: VisionKit.RecognizedItem.Bounds) -> Bounds
    {
        return Bounds.init(topLeft: bounds.topLeft, topRight: bounds.topRight, bottomRight: bounds.bottomRight, bottomLeft: bounds.bottomLeft)
    }

    @objc
    public init(topLeft: CGPoint, topRight: CGPoint, bottomRight: CGPoint, bottomLeft: CGPoint) {
        self.topLeft = topLeft
        self.topRight = topRight
        self.bottomRight = bottomRight
        self.bottomLeft = bottomLeft
    }
}
