using Plugin.Scanner.Android;
using AActivity = Android.App.Activity;

namespace Plugin.Scanner.Uno.Android;

internal class CurrentActivity : ICurrentActivity
{
    public AActivity Activity
    {
        get
        {
            if (ContextHelper.Current is not AActivity)
            {
                throw new NotSupportedException("Current activity can't be null here.");
            }

            return (AActivity)ContextHelper.Current;
        }
    }
}
