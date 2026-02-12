namespace Plugin.Scanner.Uno.Hosting;

/// <summary>
/// Provides extension methods for configuring the application builder in Uno-based projects.
/// </summary>
public static partial class UnoBuilderExtensions
{
    /// <summary>
    /// Configures the application to use the scanner plugin by adding necessary services and configurations
    /// to the specified <see cref="IApplicationBuilder"/> instance.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance to which the scanner plugin is added.
    /// </param>
    /// <returns>
    /// The modified <see cref="IApplicationBuilder"/> instance with the scanner plugin enabled.
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
    public static partial IApplicationBuilder UseScanner(this IApplicationBuilder builder);
}
