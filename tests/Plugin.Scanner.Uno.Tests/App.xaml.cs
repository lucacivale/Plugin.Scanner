using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plugin.Scanner.Uno.Hosting;
using Plugin.Scanner.Uno.Tests.Presentation;

[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]

namespace Plugin.Scanner.Uno.Tests;

// ReSharper disable once RedundantExtendsListEntry
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    private Window? MainWindow { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    protected IHost? Host { get; private set; }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    // ReSharper disable once AsyncVoidMethod
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        IApplicationBuilder builder = this.CreateBuilder(args)
            .UseToolkitNavigation()
            .UseScanner()
            .Configure(host => host
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault:true),
                ]
            )
        );
    }
}
