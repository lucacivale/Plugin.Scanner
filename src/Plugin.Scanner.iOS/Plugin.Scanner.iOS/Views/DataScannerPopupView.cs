using ObjCRuntime;

namespace Plugin.Scanner.iOS.Views;

internal sealed class DataScannerPopupView : UIView
{
    private readonly UIPanGestureRecognizer _panGestureRecognizer;

    public DataScannerPopupView()
    {
        _panGestureRecognizer = new UIPanGestureRecognizer(OnPan);

        Alpha = 0;
        BackgroundColor = UIColor.Black;
        Layer.CornerRadius = 16;
        Layer.ShadowOpacity = 0.3f;
        Layer.ShadowRadius = 10;
        Layer.ShadowOffset = new CGSize(0, 4);
        AddGestureRecognizer(_panGestureRecognizer);
    }

    public override void RemoveFromSuperview()
    {
        RemoveGestureRecognizer(_panGestureRecognizer);

        base.RemoveFromSuperview();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _panGestureRecognizer.Dispose();
        }
    }

    private void OnPan(UIPanGestureRecognizer pan)
    {
        CGPoint translation = pan.TranslationInView(Superview);

        Center = new CGPoint(Center.X + translation.X, Center.Y + translation.Y);

        pan.SetTranslation(CGPoint.Empty, Superview);

        ClampToSafeArea();
    }

    private void ClampToSafeArea()
    {
        if (Superview is null)
        {
            return;
        }

        CGRect safe = Superview.SafeAreaLayoutGuide.LayoutFrame;
        CGRect frame = Frame;

        frame.X = NMath.Max(safe.Left, NMath.Min(frame.X, safe.Right - frame.Width));

        frame.Y = NMath.Max(safe.Top, NMath.Min(frame.Y, safe.Bottom - frame.Height));

        Frame = frame;
    }
}
