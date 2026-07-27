using Android.Runtime;
using Avalonia.Android;

namespace Plugin.Scanner.Avalonia.Tests.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }
}
