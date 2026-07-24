#if ANDROID
using Plugin.Scanner.Android;
using AAViewGroup = Android.Views.ViewGroup;
#endif
using Plugin.Scanner.Maui.Tests.ViewModels;
using Plugin.Scanner.Options;

namespace Plugin.Scanner.Maui.Tests.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

	}

    private async void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
		BarcodeScanOptions options = new BarcodeScanOptions()
		{
			Formats = Core.Models.Enums.BarcodeFormat.All,
			//RegionOfInterest = new CenteredPopupRegionOfInterest(),
		};

        await Task.Delay(2000);
#if ANDROID
		new Scanner.Android.Controllers.BarcodeScannerPopupController(Handler.GetRequiredService<ICurrentActivity>()).Add((AAViewGroup)Handler!.PlatformView, options);
#endif
    }
}
