using Plugin.Scanner.Core.Controllers;

// ReSharper disable once CheckNamespace
namespace Plugin.Scanner.Core;

/// <summary>
/// Defines an overlay component for scanner views.
/// </summary>
public partial interface IOverlay
{
    /// <summary>
    /// Initializes the overlay with the specified view controller.
    /// </summary>
    /// <param name="controller">The scanner controller.</param>
    /// <param name="root">The root view to attach the overlay to.</param>
    void Init(IDataScannerController controller, UIView root);
}
