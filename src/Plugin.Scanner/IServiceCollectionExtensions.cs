using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Scanner;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
// ReSharper disable once InconsistentNaming
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds barcode scanner to container.
    /// </summary>
    /// <param name="serviceCollection">Container to add scanner.</param>
    /// <returns>Container with added scanner.</returns>
    public static partial IServiceCollection AddBarcodeScanner(this IServiceCollection serviceCollection);
}