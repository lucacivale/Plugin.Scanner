using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Scanner;

/// <summary>
/// Provides extension methods for registering barcode scanner services with the dependency injection container.
/// </summary>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers the platform-specific barcode scanner implementation with the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the barcode scanner to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    public static partial IServiceCollection AddBarcodeScanner(this IServiceCollection serviceCollection);
}