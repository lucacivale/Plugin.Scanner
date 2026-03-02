using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Scanner.Core.Scanners;
using BarcodeScanner = Plugin.Scanner.iOS.Scanners.BarcodeScanner;
using DocumentScanner = Plugin.Scanner.iOS.Scanners.DocumentScanner;
using TextScanner = Plugin.Scanner.iOS.Scanners.TextScanner;

namespace Plugin.Scanner.Hosting;

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
    public static partial IServiceCollection AddScanner(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IBarcodeScanner, BarcodeScanner>();
        serviceCollection.TryAddSingleton<ITextScanner, TextScanner>();
        serviceCollection.TryAddSingleton<IDocumentScanner, DocumentScanner>();

        return serviceCollection;
    }
}