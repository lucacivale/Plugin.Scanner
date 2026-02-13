using Microsoft.Extensions.Logging;
using Plugin.Scanner.Maui.Hosting;
using Plugin.Scanner.Maui.Tests.ViewModels;

namespace Plugin.Scanner.Maui.Tests;

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
#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<MainViewModel>();

		builder.UseScanner();

		return builder.Build();
	}
}
