namespace Plugin.Scanner.Uno.Hosting;

/// <summary>
/// Provides extension methods for configuring barcode scanner services in Uno Platform applications.
/// </summary>
public static partial class UnoBuilderExtensions
{
    /// <summary>
    /// Registers the barcode scanner services required for .NET platforms (Windows, macOS, Linux).
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The configured <see cref="IApplicationBuilder"/>.</returns>
    /// <example>
    /// <code>
    /// protected override void OnLaunched(LaunchActivatedEventArgs args)
    /// {
    ///     var builder = this.CreateBuilder(args)
    ///         .UseScanner();
    /// }
    /// </code>
    /// </example>
    public static partial IApplicationBuilder UseScanner(this IApplicationBuilder builder)
    {
        return builder.Configure(host =>
        {
            host.ConfigureServices((_, services) =>
            {
                services
                    .AddBarcodeScanner();
            });
        });
    }
}
