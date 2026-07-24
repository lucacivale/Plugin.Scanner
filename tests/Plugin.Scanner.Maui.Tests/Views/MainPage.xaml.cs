using Plugin.Scanner.Maui.Tests.ViewModels;

namespace Plugin.Scanner.Maui.Tests.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
