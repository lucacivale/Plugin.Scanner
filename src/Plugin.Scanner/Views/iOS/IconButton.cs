namespace Plugin.Scanner.Views.iOS;

internal class IconButton : UIButton
{
    protected UIImageSymbolConfiguration? _buttonSymbolConfiguration;

    public IconButton(string image)
    {
        Init(image);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _buttonSymbolConfiguration?.Dispose();
        }
    }

    private void Init(string image)
    {
        _buttonSymbolConfiguration = UIImageSymbolConfiguration.Create(22, UIImageSymbolWeight.Medium);

        SetImage(UIImage.GetSystemImage(image, _buttonSymbolConfiguration), UIControlState.Normal);

        TintColor = UIColor.White;
        TranslatesAutoresizingMaskIntoConstraints = false;
    }
}
