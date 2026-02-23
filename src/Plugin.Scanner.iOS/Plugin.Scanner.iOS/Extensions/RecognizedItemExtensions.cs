using Plugin.Scanner.iOS.Binding;

namespace Plugin.Scanner.iOS.Extensions;

internal static class RecognizedItemExtensions
{
    public static Core.Models.RecognizedItem ToRecognizedItem(this RecognizedItem item)
    {
        CGRect rect = new(item.Bounds.BottomLeft, new(item.Bounds.TopRight.X - item.Bounds.TopLeft.X, item.Bounds.TopRight.Y - item.Bounds.BottomRight.Y));

        return new(new Guid(item.Id.GetBytes()), item.Value, new((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
    }
}
