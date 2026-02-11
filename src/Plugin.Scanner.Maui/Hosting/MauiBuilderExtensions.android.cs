using Plugin.Scanner.Maui.Android;

namespace Plugin.Scanner.Maui.Hosting;

public static class MauiBuilderExtensions
{
    public static MauiAppBuilder UseScanner(this MauiAppBuilder app)
    {
        app.Services
            .AddBarcodeScanner()
            .AddCurrentActivity<CurrentActivity>();

        return app;
    }
}
