namespace Plugin.Scanner.Maui.Hosting;

/// <summary>
/// Provides extension methods for configuring the scanner plugin in a MAUI application.
/// </summary>
public static partial class MauiBuilderExtensions
{
    /// <summary>
    /// Configures the barcode scanner services for iOS platform.
    /// </summary>
    /// <param name="app">The MAUI app builder to configure.</param>
    /// <returns>The <see cref="MauiAppBuilder"/> for method chaining.</returns>
    /// <example>
    /// <code>
    /// public static MauiApp CreateMauiApp()
    /// {
    ///     var builder = MauiApp.CreateBuilder();
    ///     builder.UseScanner();
    ///     return builder.Build();
    /// }
    /// </code>
    /// </example>
    public static partial MauiAppBuilder UseScanner(this MauiAppBuilder app)
    {
        app.Services
            .AddBarcodeScanner();

        return app;
    }
}
