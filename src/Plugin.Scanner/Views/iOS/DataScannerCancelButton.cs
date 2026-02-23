namespace Plugin.Scanner.Views.iOS;

internal class DataScannerCancelButton : UIButton
{
    public DataScannerCancelButton(UIButtonType type)
        : base(type)
    {

    }

    public override void WillMoveToWindow(UIWindow? window)
    {
        base.WillMoveToWindow(window);

        Animate(
            duration: 0.10,
            animation: () =>
            {
                Alpha = 1;
            });
    }
}
