using Plugin.Scanner.Uno.Android;

namespace Plugin.Scanner.Uno.Hosting;

public static class UnoBuilderExtensions
{
    public static IApplicationBuilder UseScanner(this IApplicationBuilder builder)
    {
        return builder.Configure(host =>
        {
            host.ConfigureServices((_, services) =>
            {
                services
                    .AddBarcodeScanner()
                    .AddCurrentActivity<CurrentActivity>();
            });
        });
    }
}
