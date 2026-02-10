using AndroidX.AppCompat.App;
using Microsoft.Extensions.Logging;
using Plugin.Scanner;

#if ANDROID
using Plugin.Scanner.Android;
#endif

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddBarcodeScanner();
#if DEBUG
        builder.Logging.AddDebug();
#endif

#if ANDROID
        builder.Services.AddCurrentActivity(_ => new CurrentActivity(() => (AppCompatActivity)Platform.CurrentActivity));
#endif
        return builder.Build();
    }
}