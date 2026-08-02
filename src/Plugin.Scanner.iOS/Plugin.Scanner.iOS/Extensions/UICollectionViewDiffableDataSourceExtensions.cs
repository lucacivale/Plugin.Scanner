using System.Diagnostics.CodeAnalysis;

namespace Plugin.Scanner.iOS.Extensions;

internal static class UICollectionViewDiffableDataSourceExtensions
{
    public static bool TryGetItem<SectionIdentifierType, ItemIdentifierType>(
        this UICollectionViewDiffableDataSource<SectionIdentifierType, ItemIdentifierType> dataSource,
        [NotNullWhen(true)] out ItemIdentifierType? item,
        NSIndexPath indexPath)
        where SectionIdentifierType : NSObject
        where ItemIdentifierType : NSObject
    {
        item = dataSource.GetItemIdentifier(indexPath);

        return item is not null;
    }
}
