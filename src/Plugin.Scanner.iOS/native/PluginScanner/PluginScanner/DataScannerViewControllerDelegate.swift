@objc
@MainActor
public protocol DataScannerViewControllerDelegate: NSObjectProtocol {

    @objc(didZoom:)
    optional func dataScannerDidZoom(_ dataScanner: DataScannerViewController)

    @objc(didTapOn:didTapOn:)
    optional func dataScanner(_ dataScanner: DataScannerViewController,
                              didTapOn item: RecognizedItem)

    @objc(didAdd:didAdd:allItems:)
    optional func dataScanner(_ dataScanner: DataScannerViewController,
                              didAdd addedItems: [RecognizedItem],
                              allItems: [RecognizedItem])

    @objc(didUpdate:didUpdate:allItems:)
    optional func dataScanner(_ dataScanner: DataScannerViewController,
                              didUpdate updatedItems: [RecognizedItem],
                              allItems: [RecognizedItem])

    @objc(didRemove:didRemove:allItems:)
    optional func dataScanner(_ dataScanner: DataScannerViewController,
                              didRemove removedItems: [RecognizedItem],
                              allItems: [RecognizedItem])

    @objc(becameUnavailableWithError:becameUnavailableWithError:)
    optional func dataScanner(_ dataScanner: DataScannerViewController,
                              becameUnavailableWithError error: ScanningUnavailable)
}
