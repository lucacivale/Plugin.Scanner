using System.Globalization;

namespace Plugin.Scanner.iOS.Extensions;

internal static class UICollectionViewListCellExtensions
{
    public static void ConfigureCell(this UICollectionViewListCell cell, DataScannerResultsViewController.ListItem item)
    {
        const float contentSpacing = 12f;
        const float fontSize = 17f;
        const string finderIcon = "ellipsis.viewfinder";

        UIListContentConfiguration content = UIListContentConfiguration.SubtitleCellConfiguration;
        content.Text = item.Value;
        content.SecondaryText = item.Timestamp.ToString("t", CultureInfo.CurrentCulture);
        content.Image = UIImage.GetSystemImage(finderIcon);
        content.ImageToTextPadding = contentSpacing;
        content.TextToSecondaryTextVerticalPadding = 3;
        content.DirectionalLayoutMargins = new NSDirectionalEdgeInsets(14, 16, 14, 16);

        UIListContentTextProperties text = content.TextProperties;
        text.Font = UIFontMetrics.GetMetrics(UIFontTextStyle.Body.GetConstant()!).GetScaledFont(UIFont.GetMonospacedSystemFont(fontSize, UIFontWeight.Medium));
        text.Color = UIColor.Label;
        text.NumberOfLines = 2;
        text.LineBreakMode = UILineBreakMode.TailTruncation;
        text.AdjustsFontForContentSizeCategory = true;

        UIListContentTextProperties secondary = content.SecondaryTextProperties;
        secondary.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Footnote);
        secondary.Color = UIColor.SecondaryLabel;
        secondary.NumberOfLines = 1;
        secondary.AdjustsFontForContentSizeCategory = true;

        UIListContentImageProperties image = content.ImageProperties;
        image.TintColor = UIColor.SystemGreen;
        image.PreferredSymbolConfiguration = UIImageSymbolConfiguration.Create(UIFontTextStyle.Title3);

        cell.ContentConfiguration = content;
        cell.AutomaticallyUpdatesBackgroundConfiguration = false;
        cell.BackgroundConfiguration = cell.CreateCardBackground(item.HighlightOnAppear);
        cell.ClipsToBounds = true;
        cell.Layer.CornerRadius = 14f;

        cell.IsAccessibilityElement = true;
        cell.AccessibilityLabel = $"{item.Value}, {content.SecondaryText}";
        cell.AccessibilityHint = "Double tap to copy. Swipe for more actions.";

        if (item.HighlightOnAppear == false)
        {
            return;
        }

        // The flash is consumed by the first cell that renders the item, so a later reuse stays calm.
        item.HighlightOnAppear = false;

        cell.AnimateFlash();
    }

    private static void AnimateFlash(this UICollectionViewListCell cell)
    {
        const float flashHoldDuration = 0.5f;
        const float flashFadeDuration = 0.55f;

        UIView.Animate(
            flashFadeDuration,
            flashHoldDuration,
            UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.AllowUserInteraction,
            () =>
            {
                cell.BackgroundConfiguration = cell.CreateCardBackground(false);
                cell.LayoutIfNeeded();
            },
            () => { });
    }

    private static UIBackgroundConfiguration CreateCardBackground(this UICollectionViewListCell cell, bool highlighted)
    {
        const float cardCornerRadius = 14f;

        UIBackgroundConfiguration background;

        if (OperatingSystem.IsIOSVersionAtLeast(18))
        {
            // iOS 18 folded the grouped variant into the general list configuration.
            background = UIBackgroundConfiguration.ListCellConfiguration;
        }
        else
        {
            background = UIBackgroundConfiguration.ListGroupedCellConfiguration;
        }

        UIColor flashColor =
            UIColor.FromDynamicProvider(traits => traits.UserInterfaceStyle == UIUserInterfaceStyle.Dark
            ? UIColor.SystemGreen.ColorWithAlpha(0.32f)
            : UIColor.SystemGreen.ColorWithAlpha(0.18f));
        background.BackgroundColor = highlighted ? flashColor : UIColor.SecondarySystemGroupedBackground;
        background.CornerRadius = cardCornerRadius;
        background.StrokeColor = highlighted ? UIColor.SystemGreen : UIColor.Separator;
        background.StrokeWidth = highlighted ? 1.5f : 0.5f;

        return background;
    }
}
