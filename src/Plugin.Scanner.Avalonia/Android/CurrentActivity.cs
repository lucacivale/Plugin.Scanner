using Plugin.Scanner.Android;

namespace Plugin.Scanner.Avalonia.Android;

internal class CurrentActivity : ICurrentActivity
{
    public Activity Activity => Platform.CurrentActivity ?? throw new NotSupportedException("Current activity can't be null here.");
}
