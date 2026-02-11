namespace Plugin.Scanner.Maui.Hosting;

public static class MauiBuilderExtensions
{
    public static MauiAppBuilder UseScanner(this MauiAppBuilder app)
    {
        app.Services
            .AddBarcodeScanner();

        return app;
    }
}
