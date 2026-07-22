using Plugin.Scanner.Core.Controllers;

// ReSharper disable once CheckNamespace
namespace Plugin.Scanner.Core;

/// <summary>
/// Defines an overlay component for scanner dialogs.
/// </summary>
public partial interface IOverlay
{
    /// <summary>
    /// Initializes the overlay with the specified dialog and root view.
    /// </summary>
    /// <param name="controller">The scanner controller.</param>
    /// <param name="root">The root view to attach the overlay to.</param>
    void Init(IDataScannerController controller, View root);
}