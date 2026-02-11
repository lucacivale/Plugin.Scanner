using UIKit;
using Uno.UI.Hosting;

namespace Plugin.Scanner.Uno.Tests.iOS;

public class EntryPoint
{
    // This is the main entry point of the application.
    public static void Main(string[] args)
    {
        UnoPlatformHost host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseAppleUIKit()
            .Build();

        host.Run();
    }
}
