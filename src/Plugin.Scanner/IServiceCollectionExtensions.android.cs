using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Scanner.Core.Barcode;

namespace Plugin.Scanner;

// ReSharper disable once InconsistentNaming
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds barcode scanner to container.
    /// </summary>
    /// <param name="serviceCollection">Container to add scanner.</param>
    /// <returns>Container with added scanner.</returns>
    public static partial IServiceCollection AddBarcodeScanner(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IBarcodeScanner, Plugin.Scanner.Android.Barcode.BarcodeScanner>();

        return serviceCollection;
    }
}