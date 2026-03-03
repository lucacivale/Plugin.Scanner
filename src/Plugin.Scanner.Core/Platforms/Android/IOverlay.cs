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
    /// <param name="dialog">The dialog containing the overlay.</param>
    /// <param name="root">The root view to attach the overlay to.</param>
    void Init(Dialog dialog, View root);
}