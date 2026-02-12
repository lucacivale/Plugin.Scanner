namespace Plugin.Scanner.Maui.Hosting;

/// <summary>
/// Provides extension methods for configuring the Plugin.Scanner library within a .NET MAUI application.
/// </summary>
public static partial class MauiBuilderExtensions
{
    /// <summary>
    /// Registers and configures the scanner plugin for use in the MAUI application.
    /// </summary>
    /// <param name="app">
    /// An instance of <see cref="MauiAppBuilder"/> representing the application builder to which the scanner plugin is added.
    /// </param>
    /// <returns>
    /// The <see cref="MauiAppBuilder"/> instance, allowing for method chaining.
    /// </returns>
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
    public static partial MauiAppBuilder UseScanner(this MauiAppBuilder app);
}
