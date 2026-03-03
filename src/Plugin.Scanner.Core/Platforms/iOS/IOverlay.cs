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
    /// <param name="viewController">The view controller containing the overlay.</param>
    void Init(UIViewController viewController);
}