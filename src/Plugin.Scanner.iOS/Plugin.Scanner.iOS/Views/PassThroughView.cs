namespace Plugin.Scanner.iOS.Views;

internal sealed class PassThroughView : UIView
{
    public PassThroughView()
    {
        BackgroundColor = UIColor.Clear;
    }

    public override UIView? HitTest(CGPoint point, UIEvent? uievent)
    {
        UIView? hit = base.HitTest(point, uievent);

        return hit?.Equals(this) == true ? null : hit;
    }
}
