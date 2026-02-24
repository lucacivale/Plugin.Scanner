using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;
using Plugin.Scanner.Core;
using ContentView = Microsoft.Maui.Controls.ContentView;

#if IOS
using UIKit;
#endif

#if ANDROID
using Android.App;
using Android.Views;
using View = Android.Views.View;
#endif

namespace Plugin.Scanner.Maui.Tests;

public class BarcodeCustomOverlay : IOverlay
{
    #if IOS
    private UIViewController? _viewController;
    #endif
    
    #if ANDROID
    private View? _root;
    #endif

    public void Dispose()
    {
    }

    public void AddOverlay()
    {
        Border boxView = new()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Padding = new Thickness(20),
            Background = new SolidColorBrush(Colors.Transparent),
            Stroke = new SolidColorBrush(Colors.Green),
            StrokeThickness = 5,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20),
            },
            Content = new Label { Text = "I'm a custom overlay!" }
        };

        VerticalStackLayout stack = new()
        {
            Margin = new Thickness(0, 100)
        };
        stack.Add(boxView);

        IMauiContext? context = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext;
        
        if (context is null)
        {
            return;
        }

        #if IOS
        _viewController?.View?.Add(new ContentView { Content = stack }.ToPlatform(context));
#endif

#if ANDROID
        if (_root is ViewGroup viewGroup)
        {
            View platformView = new ContentView {Content = stack }.ToPlatform(context);

            viewGroup.AddView(platformView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
#endif
    }

    public void AddRegionOfInterest(IRegionOfInterest? regionOfInterest)
    {
        Border boxView = new()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            StrokeThickness = 5,
            Padding = new Thickness(20),
            Background = new SolidColorBrush(Colors.Transparent),
            Stroke = new SolidColorBrush(Colors.Blue),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20),
            },
            Content = new Label { Text = "I'm a custom region of interest!" }
        };

        Grid grid = new()
        {
            Margin = new Thickness(0, 100)
        };
        grid.Add(boxView);

        IMauiContext? context = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext;
        
        if (context is null)
        {
            return;
        }

        #if IOS
        _viewController?.View?.Add(new ContentView {Content = grid }.ToPlatform(context));
        #endif
        
        #if ANDROID
        if (_root is ViewGroup viewGroup)
        {
            View platformView = new ContentView {Content = grid }.ToPlatform(context);

            viewGroup.AddView(platformView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
        #endif
    }

    public void Cleanup()
    {
    }

    #if IOS
    public void Init(UIViewController viewController)
    {
        _viewController = viewController;
    }
    #endif

    #if ANDROID
    public void Init(Dialog dialog, View root)
    {
        _root = root;
    }
    #endif
}