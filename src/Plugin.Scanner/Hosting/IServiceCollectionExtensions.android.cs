using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Scanner.Android;
using System.Diagnostics.CodeAnalysis;
using Plugin.Scanner.Core.Scanners;
using BarcodeScanner = Plugin.Scanner.Android.Scanners.BarcodeScanner;
using TextScanner = Plugin.Scanner.Android.Scanners.TextScanner;

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

        return serviceCollection;
    }

    /// <summary>
    /// Registers a custom implementation of <see cref="ICurrentActivity"/> with the service collection.
    /// </summary>
    /// <typeparam name="TCurrentActivityImplementation">The type that implements <see cref="ICurrentActivity"/>.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddCurrentActivity<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCurrentActivityImplementation>(this IServiceCollection serviceCollection)
        where TCurrentActivityImplementation : class, ICurrentActivity
    {
        serviceCollection.TryAddSingleton<ICurrentActivity, TCurrentActivityImplementation>();

        return serviceCollection;
    }
}