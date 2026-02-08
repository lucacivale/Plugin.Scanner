using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Scanner;

/// <summary>
/// Provides extension methods for registering barcode scanner services with the dependency injection container.
/// </summary>
/// <remarks>
/// This partial class contains platform-specific implementations that register the appropriate
/// barcode scanner implementation based on the target platform.
/// </remarks>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers the platform-specific barcode scanner implementation with the service collection.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the barcode scanner to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method registers <see cref="Plugin.Scanner.Core.Barcode.IBarcodeScanner"/> as a singleton service
    /// with the appropriate platform-specific implementation:
    /// <list type="bullet">
    /// <item><description>iOS: <c>Plugin.Scanner.iOS.Barcode.BarcodeScanner</c></description></item>
    /// <item><description>Android: <c>Plugin.Scanner.Android.Barcode.BarcodeScanner</c></description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If a barcode scanner is already registered, this method will not replace it (uses <c>TryAddSingleton</c>).
    /// </para>
    /// </remarks>
    /// <example>
    /// Register the barcode scanner in your application startup:
    /// <code>
    /// public static class MauiProgram
    /// {
    ///     public static MauiApp CreateMauiApp()
    ///     {
    ///         var builder = MauiApp.CreateBuilder();
    ///         builder.Services.AddBarcodeScanner();
    ///         return builder.Build();
    ///     }
    /// }
    /// </code>
    /// </example>
    public static partial IServiceCollection AddBarcodeScanner(this IServiceCollection serviceCollection);
}