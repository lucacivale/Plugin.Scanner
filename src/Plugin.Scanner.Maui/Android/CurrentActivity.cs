using Android.App;
using Plugin.Scanner.Android;

namespace Plugin.Scanner.Maui.Android;

internal class CurrentActivity : ICurrentActivity
{
    public Activity Activity => Platform.CurrentActivity ?? throw new NotSupportedException("Current activity can't be null here.");
}
