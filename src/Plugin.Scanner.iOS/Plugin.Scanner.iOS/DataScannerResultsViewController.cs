using ObjCRuntime;
using Plugin.Scanner.Core.Models;
using Plugin.Scanner.iOS.Extensions;

namespace Plugin.Scanner.iOS;

internal sealed class DataScannerResultsViewController : UIViewController
{
    private const string PeekDetentId = "Plugin.Scanner.BottomSheetUIViewController.Peek";

    private const string CopyIcon = "doc.on.doc";
    private const string ShareIcon = "square.and.arrow.up";
    private const string EditIcon = "pencil";
    private const string DeleteIcon = "trash";

    private const float ToastOffset = 24f;
    private const float ToastBottomInset = 24f;
    private const float CardCornerRadius = 14f;
    private const float CardSpacing = 10f;
    private const float ContentSpacing = 12f;

    private readonly UISheetPresentationControllerDetent _peekDetent;
    private readonly UISheetPresentationControllerDetent _largeDetent = UISheetPresentationControllerDetent.CreateLargeDetent();

    private readonly List<ListItem> _items = [];
    private readonly List<ListItem> _visibleItems = [];

    private readonly NSString _sectionIdentifier = new("Plugin.Scanner.ScanResultList.Section");
    private readonly UISelectionFeedbackGenerator _selectionFeedback = new();
    private readonly UINotificationFeedbackGenerator _notificationFeedback = new();

    private readonly EditAlertViewController _editAlert;

    private readonly UIBarButtonItem _clearButton;
    private readonly UISearchController _searchController = new();

    private readonly UICollectionLayoutListConfiguration _listConfiguration;
    private readonly UICollectionView _collectionView;
    private readonly UICollectionViewCellRegistration _cellRegistration;
    private readonly UICollectionViewDiffableDataSource<NSString, ListItem> _dataSource;
    private readonly CollectionDelegate _collectionDelegate;
    private readonly SearchDelegate _searchDelegate;

    private readonly UIStackView _emptyStateStack = new();
    private readonly UIImageView _emptyStateImage = new();
    private readonly UILabel _emptyStateTitle = new();
    private readonly UILabel _emptyStateMessage = new();

    private readonly UIVisualEffectView _toastView;
    private readonly UILabel _toastLabel = new();

    private readonly string _headerTitle = "Scanned items";

    private string _query = string.Empty;
    private bool _isEmptyStateVisible;

    public DataScannerResultsViewController()
    {
        _peekDetent = UISheetPresentationControllerDetent.Create(PeekDetentId, PeekHeightValue);
        _clearButton = new UIBarButtonItem(string.Empty, UIBarButtonItemStyle.Plain, OnClearTouched);

        _editAlert = CreateEditAlertViewController();
        _listConfiguration = CreateListConfiguration();
        _collectionView = CreateCollectionView();
        _cellRegistration = CreateCollectionViewCellRegistration();
        _dataSource = CreateDataSource();

        _collectionDelegate = new CollectionDelegate(this);
        _searchDelegate = new SearchDelegate(this);

        _toastView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.SystemThickMaterial));
    }

    public bool IsOpen { get; private set; }

    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);

        InitSheet();
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        if (View is null)
        {
            return;
        }

        View.BackgroundColor = UIColor.SystemGroupedBackground;

        ConfigureNavigation();
        ConfigureCollectionView();
        ConfigureEmptyState();
        ConfigureToast();

        View.AddSubview(_collectionView);
        View.AddSubview(_emptyStateStack);
        View.AddSubview(_toastView);

        NSLayoutConstraint.ActivateConstraints(
        [
            _collectionView.TopAnchor.ConstraintEqualTo(View.TopAnchor),
            _collectionView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
            _collectionView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
            _collectionView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),

            _emptyStateStack.CenterXAnchor.ConstraintEqualTo(_collectionView.CenterXAnchor),
            _emptyStateStack.CenterYAnchor.ConstraintEqualTo(_collectionView.CenterYAnchor),
            _emptyStateStack.LeadingAnchor.ConstraintGreaterThanOrEqualTo(View.LayoutMarginsGuide.LeadingAnchor),
            _emptyStateStack.TrailingAnchor.ConstraintLessThanOrEqualTo(View.LayoutMarginsGuide.TrailingAnchor),

            _toastView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
            _toastView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -ToastBottomInset),
            _toastView.LeadingAnchor.ConstraintGreaterThanOrEqualTo(View.LayoutMarginsGuide.LeadingAnchor),
            _toastView.TrailingAnchor.ConstraintLessThanOrEqualTo(View.LayoutMarginsGuide.TrailingAnchor),

            _toastLabel.TopAnchor.ConstraintEqualTo(_toastView.ContentView.TopAnchor, ContentSpacing - 2),
            _toastLabel.BottomAnchor.ConstraintEqualTo(_toastView.ContentView.BottomAnchor, -(ContentSpacing - 2)),
            _toastLabel.LeadingAnchor.ConstraintEqualTo(_toastView.ContentView.LeadingAnchor, CardCornerRadius + 2),
            _toastLabel.TrailingAnchor.ConstraintEqualTo(_toastView.ContentView.TrailingAnchor, -(CardCornerRadius + 2)),
        ]);

        ApplySnapshot(false, []);
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();

        (NavigationController?.SheetPresentationController ?? SheetPresentationController)?.InvalidateDetents();
    }

    public void Add(RecognizedItem item)
    {
        ListItem listItem = new(item.Text, DateTime.Now);
        listItem.HighlightOnAppear = true;
        _items.Insert(0, listItem);

        _selectionFeedback.Prepare();
        _selectionFeedback.SelectionChanged();

        ApplySnapshot(true, []);
        ScrollToTop(listItem);
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);

        IsOpen = true;
    }

    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);

        IsOpen = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _largeDetent.Dispose();
            _peekDetent.Dispose();

            _searchController.SearchBar.Delegate = null!;
            _collectionView.Delegate = null!;
            NavigationItem.SearchController = null;

            _searchDelegate.Dispose();
            _collectionDelegate.Dispose();
            _dataSource.Dispose();
            _cellRegistration.Dispose();
            _selectionFeedback.Dispose();
            _notificationFeedback.Dispose();

            _clearButton.Dispose();
            _searchController.Dispose();
            _collectionView.Dispose();
            _listConfiguration.Dispose();

            _emptyStateImage.Dispose();
            _emptyStateTitle.Dispose();
            _emptyStateMessage.Dispose();
            _emptyStateStack.Dispose();

            _toastLabel.Dispose();
            _toastView.Dispose();

            _sectionIdentifier.Dispose();
            _editAlert.Dispose();
        }

        base.Dispose(disposing);
    }

    private static UIContextualAction CreateContextAction(
        UIContextualActionStyle style,
        string title,
        UIContextualActionHandler handler)
    {
        UIContextualAction action = UIContextualAction.FromContextualActionStyle(
            style,
            title,
            handler);

        return action;
    }

    private static nfloat PeekHeightValue(IUISheetPresentationControllerDetentResolutionContext arg)
    {
        return arg.MaximumDetentValue * 0.45f;
    }

    private UICollectionLayoutListConfiguration CreateListConfiguration()
    {
        UICollectionLayoutListConfiguration listConfiguration = new(UICollectionLayoutListAppearance.InsetGrouped);
        listConfiguration.ShowsSeparators = false;
        listConfiguration.HeaderMode = UICollectionLayoutListHeaderMode.None;
        listConfiguration.BackgroundColor = UIColor.Clear;
        listConfiguration.LeadingSwipeActionsConfigurationProvider = CreateLeadingSwipeActions;
        listConfiguration.TrailingSwipeActionsConfigurationProvider = CreateTrailingSwipeActions;

        return listConfiguration;
    }

    private UISwipeActionsConfiguration? CreateLeadingSwipeActions(NSIndexPath indexPath)
    {
        if (_dataSource.TryGetItem(out ListItem? item, indexPath) == false)
        {
            return null;
        }

        using UIContextualAction copy = CreateContextAction(
            UIContextualActionStyle.Normal,
            string.Empty,
            (_, _, completion) =>
            {
                Copy(item);
                completion(true);
            });

        copy.BackgroundColor = UIColor.SystemGreen;
        copy.Image = UIImage.GetSystemImage(CopyIcon);

        UISwipeActionsConfiguration configuration = UISwipeActionsConfiguration.FromActions([copy]);

        // Keep both sides as action menus. Full-swipe behaves like a commit gesture and
        // otherwise makes the single leading action animate differently from this side.
        configuration.PerformsFirstActionWithFullSwipe = false;

        return configuration;
    }

    private UISwipeActionsConfiguration? CreateTrailingSwipeActions(NSIndexPath indexPath)
    {
        if (_dataSource.TryGetItem(out ListItem? item, indexPath) == false)
        {
            return null;
        }

        using UIContextualAction delete = CreateContextAction(
            UIContextualActionStyle.Destructive,
            string.Empty,
            (_, _, completion) =>
            {
                Delete(item);
                completion(true);
            });
        delete.Image = UIImage.GetSystemImage(DeleteIcon);

        using UIContextualAction edit = CreateContextAction(
            UIContextualActionStyle.Normal,
            string.Empty,
            (_, _, completion) =>
            {
                Edit(item);
                completion(true);
            });
        edit.BackgroundColor = UIColor.SystemIndigo;
        edit.Image = UIImage.GetSystemImage(EditIcon);

        UISwipeActionsConfiguration configuration = UISwipeActionsConfiguration.FromActions([delete, edit]);
        configuration.PerformsFirstActionWithFullSwipe = false;

        return configuration;
    }

    private void Copy(ListItem item)
    {
        const int toastValueLimit = 28;

        UIPasteboard.General.String = item.Value;

        _notificationFeedback.Prepare();
        _notificationFeedback.NotificationOccurred(UINotificationFeedbackType.Success);

        ShowToast($"Copied {item.Value.Truncate(toastValueLimit)}");
    }

    private void Delete(ListItem item)
    {
        if (_items.Remove(item) == false)
        {
            return;
        }

        _notificationFeedback.Prepare();
        _notificationFeedback.NotificationOccurred(UINotificationFeedbackType.Warning);

        ApplySnapshot(true, []);
        ShowToast("Deleted");
    }

    private void Edit(ListItem item)
    {
        _editAlert.Item = item;
        _editAlert.Value = item.Value;

        PresentViewController(_editAlert.Alert, true, null);
    }

    private bool Remove(ListItem item)
    {
        bool removed = false;

        int index = _items.FindIndex(existing => ReferenceEquals(existing, item));

        if (index < 0)
        {
            return removed;
        }

        _items.RemoveAt(index);

        ApplySnapshot(true, []);

        return removed;
    }

    private void Share(ListItem item, UIView source)
    {
        UIActivityViewController activity = new([new NSString(item.Value)], null);

        if (activity.PopoverPresentationController is { } popover)
        {
            popover.SourceView = source;
            popover.SourceRect = source.Bounds;
        }

        PresentViewController(activity, true, null);
    }

    private void Clear()
    {
        if (_items.Count == 0)
        {
            return;
        }

        _items.Clear();

        ApplySnapshot(true, []);
    }

    private void Update(ListItem item, string value)
    {
        item.Value = value;

        ApplySnapshot(true, [item]);
    }

    private void ApplySnapshot(bool animated, IReadOnlyList<ListItem> reconfigure)
    {
        _visibleItems.Clear();
        _visibleItems.AddRange(Filter());

        using NSDiffableDataSourceSnapshot<NSString, ListItem> snapshot = new();

        snapshot.AppendSections([_sectionIdentifier]);
        snapshot.AppendItems(_visibleItems.ToArray());

        if (reconfigure.Any())
        {
            ListItem[] pending = reconfigure.Where(_visibleItems.Contains).ToArray();

            if (pending.Length != 0)
            {
                snapshot.ReconfigureItems(pending);
            }
        }

        _dataSource.ApplySnapshot(snapshot, animated);

        UpdateHeader();
        UpdateEmptyState();
    }

    private void UpdateHeader()
    {
        NavigationItem.Title = _headerTitle;
        _clearButton.Enabled = _items.Count > 0;
        NavigationItem.RightBarButtonItem = _items.Count > 0 ? _clearButton : null;
    }

    private void UpdateEmptyState()
    {
        const double emptyStateFadeDuration = 0.25;
        const string emptyStateIcon = "magnifyingglass";
        const string finderIcon = "viewfinder";

        bool shouldShow = _visibleItems.Count == 0;

        if (_isEmptyStateVisible == shouldShow)
        {
            return;
        }

        bool searching = string.IsNullOrWhiteSpace(_query) == false;

        _emptyStateImage.Image = UIImage.GetSystemImage(searching ? emptyStateIcon : finderIcon);
        _emptyStateTitle.Text = searching ? "No matches" : "No items yet";
        _emptyStateMessage.Text = searching ? $"Nothing here matches “{_query}”." : "Scanned items are collected here, newest first.";

        _isEmptyStateVisible = shouldShow;

        nfloat target = shouldShow ? 1 : 0;

        UIView.Animate(emptyStateFadeDuration, () => _emptyStateStack.Alpha = target);
    }

    private IEnumerable<ListItem> Filter()
    {
        if (string.IsNullOrWhiteSpace(_query))
        {
            return _items;
        }

        return _items.Where(item => item.Value.Contains(_query, StringComparison.CurrentCultureIgnoreCase));
    }

    private void ShowToast(string message)
    {
        const double toastVisibleDuration = 1.5;

        _toastLabel.Text = message;
        _toastView.Hidden = false;

        UIView.Animate(
            toastVisibleDuration,
            () =>
            {
                _toastView.Alpha = 1;
                _toastView.Transform = CGAffineTransform.MakeIdentity();
            },
            HideToast);
    }

    private void HideToast()
    {
        const double toastFadeDuration = 0.22;

        UIView.Animate(
            toastFadeDuration,
            0,
            UIViewAnimationOptions.CurveEaseIn,
            () =>
            {
                _toastView.Alpha = 0;
                _toastView.Transform = CGAffineTransform.MakeTranslation(0, ToastOffset);
            },
            () => _toastView.Hidden = true);
    }

    private void InitSheet()
    {
        UISheetPresentationController? sheet = NavigationController?.SheetPresentationController ?? SheetPresentationController;

        sheet?.PreferredCornerRadius = 50;
        sheet?.LargestUndimmedDetentIdentifier = UISheetPresentationControllerDetentIdentifier.Large;
        sheet?.Detents = [_peekDetent, _largeDetent];
        sheet?.PrefersScrollingExpandsWhenScrolledToEdge = false;
        sheet?.PrefersEdgeAttachedInCompactHeight = false;

        if (NavigationController is { } navigationController)
        {
            navigationController.ModalInPresentation = false;
        }
        else
        {
            ModalInPresentation = false;
        }
    }

    private UICollectionViewCellRegistration CreateCollectionViewCellRegistration()
    {
        return UICollectionViewCellRegistration.GetRegistration(
            new Class(typeof(UICollectionViewListCell)),
            (cell, _, item) =>
            {
                if (cell is UICollectionViewListCell listCell
                    && item is ListItem listItem)
                {
                    listCell.ConfigureCell(listItem);
                }
            });
    }

    private UICollectionViewDiffableDataSource<NSString, ListItem> CreateDataSource()
    {
        return new UICollectionViewDiffableDataSource<NSString, ListItem>(_collectionView, (collectionView, indexPath, item) => collectionView.DequeueConfiguredReusableCell(_cellRegistration, indexPath, item));
    }

    private UICollectionView CreateCollectionView()
    {
        using UICollectionViewLayout layout = new UICollectionViewCompositionalLayout((_, environment) =>
            {
                NSCollectionLayoutSection section = NSCollectionLayoutSection.GetSection(_listConfiguration, environment);

                // Put the spacing in the layout so the cell and UIKit's swipe-action
                // container have identical bounds.
                section.InterGroupSpacing = CardSpacing;

                return section;
            });

        return new UICollectionView(CGRect.Empty, layout);
    }

    private EditAlertViewController CreateEditAlertViewController()
    {
        return new(_ =>
        {
            Update(_editAlert.Item, _editAlert.Value.Trim());
            ShowToast("Updated");
        });
    }

    private void ConfigureNavigation()
    {
        NavigationController?.NavigationBar.PrefersLargeTitles = true;
        NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Always;
        NavigationItem.SearchController = _searchController;
        NavigationItem.HidesSearchBarWhenScrolling = true;

        _clearButton.TintColor = UIColor.SystemRed;
        _clearButton.AccessibilityLabel = "Clear all items";
        _clearButton.Image = UIImage.GetSystemImage(DeleteIcon);

        _searchController.ObscuresBackgroundDuringPresentation = false;
        _searchController.SearchBar.Placeholder = "Search";
        _searchController.SearchBar.AutocorrectionType = UITextAutocorrectionType.No;
        _searchController.SearchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
        _searchController.SearchBar.SpellCheckingType = UITextSpellCheckingType.No;
        _searchController.SearchBar.Delegate = _searchDelegate;

        DefinesPresentationContext = true;

        UpdateHeader();
    }

    private void ConfigureCollectionView()
    {
        _collectionView.TranslatesAutoresizingMaskIntoConstraints = false;
        _collectionView.BackgroundColor = UIColor.Clear;
        _collectionView.AlwaysBounceVertical = true;
        _collectionView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
        _collectionView.ContentInset = new UIEdgeInsets(CardSpacing / 2, 0, ToastBottomInset * 2, 0);
        _collectionView.Delegate = _collectionDelegate;
    }

    private void ConfigureEmptyState()
    {
        _emptyStateImage.ContentMode = UIViewContentMode.ScaleAspectFit;
        _emptyStateImage.TintColor = UIColor.TertiaryLabel;
        _emptyStateImage.PreferredSymbolConfiguration = UIImageSymbolConfiguration.Create(48, UIImageSymbolWeight.Light);

        _emptyStateTitle.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Headline);
        _emptyStateTitle.AdjustsFontForContentSizeCategory = true;
        _emptyStateTitle.TextColor = UIColor.SecondaryLabel;
        _emptyStateTitle.TextAlignment = UITextAlignment.Center;
        _emptyStateTitle.Lines = 0;

        _emptyStateMessage.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Subheadline);
        _emptyStateMessage.AdjustsFontForContentSizeCategory = true;
        _emptyStateMessage.TextColor = UIColor.TertiaryLabel;
        _emptyStateMessage.TextAlignment = UITextAlignment.Center;
        _emptyStateMessage.Lines = 0;

        _emptyStateStack.Axis = UILayoutConstraintAxis.Vertical;
        _emptyStateStack.Alignment = UIStackViewAlignment.Center;
        _emptyStateStack.Spacing = ContentSpacing / 2;
        _emptyStateStack.TranslatesAutoresizingMaskIntoConstraints = false;
        _emptyStateStack.UserInteractionEnabled = false;
        _emptyStateStack.Alpha = 0;
        _emptyStateStack.AddArrangedSubview(_emptyStateImage);
        _emptyStateStack.SetCustomSpacing(ContentSpacing, _emptyStateImage);
        _emptyStateStack.AddArrangedSubview(_emptyStateTitle);
        _emptyStateStack.AddArrangedSubview(_emptyStateMessage);
    }

    private void ConfigureToast()
    {
        const float toastCornerRadius = 18f;

        _toastLabel.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Subheadline);
        _toastLabel.AdjustsFontForContentSizeCategory = true;
        _toastLabel.TextColor = UIColor.Label;
        _toastLabel.TextAlignment = UITextAlignment.Center;
        _toastLabel.Lines = 2;
        _toastLabel.TranslatesAutoresizingMaskIntoConstraints = false;

        _toastView.TranslatesAutoresizingMaskIntoConstraints = false;
        _toastView.ClipsToBounds = true;
        _toastView.Layer.CornerRadius = toastCornerRadius;
        _toastView.Alpha = 0;
        _toastView.Hidden = true;
        _toastView.UserInteractionEnabled = false;
        _toastView.Transform = CGAffineTransform.MakeTranslation(0, ToastOffset);
        _toastView.ContentView.AddSubview(_toastLabel);
    }

    private void ScrollToTop(ListItem item)
    {
        if (IsViewLoaded == false
            || _visibleItems.Count == 0
            || ReferenceEquals(_visibleItems[0], item) == false)
        {
            return;
        }

        _collectionView.ScrollToItem(NSIndexPath.FromItemSection(0, 0), UICollectionViewScrollPosition.Top, true);
    }

    private UIContextMenuConfiguration? CreateContextMenu(NSIndexPath indexPath)
    {
        if (_dataSource.TryGetItem(out ListItem? item, indexPath) == false)
        {
            return null;
        }

        UIView source = _collectionView.CellForItem(indexPath) ?? (UIView)_collectionView;

        return UIContextMenuConfiguration.Create(
            null,
            null,
            _ =>
            {
                List<UIMenuElement> elements =
                [
                    UIAction.Create(
                        "Copy",
                        UIImage.GetSystemImage(CopyIcon),
                        "copy",
                        _ => Copy(item)),
                    UIAction.Create(
                        "Share",
                        UIImage.GetSystemImage(ShareIcon),
                        "share",
                        _ => Share(item, source)),
                    UIAction.Create(
                        "Edit",
                        UIImage.GetSystemImage(EditIcon),
                        "edit",
                        _ => Edit(item))
                ];

                UIAction delete = UIAction.Create(
                    "Delete",
                    UIImage.GetSystemImage(DeleteIcon),
                    "delete",
                    _ => Delete(item));

                delete.Attributes = UIMenuElementAttributes.Destructive;

                elements.Add(delete);

                return UIMenu.Create([.. elements]);
            });
    }

    private void HandleItemSelected(NSIndexPath indexPath)
    {
        if (_dataSource.TryGetItem(out ListItem? item, indexPath) == false)
        {
            return;
        }

        Copy(item);
    }
    
    private void ApplyQuery(string query)
    {
        string trimmed = query.Trim();

        if (string.Equals(_query, trimmed, StringComparison.Ordinal))
        {
            return;
        }

        _query = trimmed;

        ApplySnapshot(true, []);
    }

    private sealed class CollectionDelegate(DataScannerResultsViewController owner) : UICollectionViewDelegate
    {
        private readonly WeakReference<DataScannerResultsViewController> _owner = new(owner);

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            collectionView.DeselectItem(indexPath, true);

            if (_owner.TryGetTarget(out DataScannerResultsViewController? controller))
            {
                controller.HandleItemSelected(indexPath);
            }
        }

        public override UIContextMenuConfiguration? GetContextMenuConfiguration(
            UICollectionView collectionView,
            NSIndexPath indexPath,
            CGPoint point)
        {
            UIContextMenuConfiguration? configuration = null;

            if (_owner.TryGetTarget(out DataScannerResultsViewController? controller))
            {
                configuration = controller.CreateContextMenu(indexPath);
            }

            return configuration;
        }
    }

    private void OnClearTouched(object? sender, EventArgs e)
    {
        if (_items.Count == 0)
        {
            return;
        }

        UIAlertController confirmation = UIAlertController.Create(
            null,
            $"Remove all {_items.Count} scanned items?",
            UIAlertControllerStyle.ActionSheet);

        confirmation.AddAction(UIAlertAction.Create("Clear all", UIAlertActionStyle.Destructive, _ =>
        {
            _notificationFeedback.Prepare();
            _notificationFeedback.NotificationOccurred(UINotificationFeedbackType.Warning);

            Clear();
        }));

        confirmation.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

        if (confirmation.PopoverPresentationController is { } popover)
        {
            popover.SourceItem = _clearButton;
        }

        PresentViewController(confirmation, true, null);
    }

    private sealed class SearchDelegate(DataScannerResultsViewController owner) : UISearchBarDelegate
    {
        private readonly WeakReference<DataScannerResultsViewController> _owner = new(owner);

        public override void TextChanged(UISearchBar searchBar, string searchText)
        {
            if (_owner.TryGetTarget(out DataScannerResultsViewController? controller))
            {
                controller.ApplyQuery(searchText);
            }
        }

        public override void OnEditingStarted(UISearchBar searchBar) => searchBar.SetShowsCancelButton(true, true);

        public override void OnEditingStopped(UISearchBar searchBar) => searchBar.SetShowsCancelButton(false, true);

        public override void SearchButtonClicked(UISearchBar searchBar) => searchBar.ResignFirstResponder();

        public override void CancelButtonClicked(UISearchBar searchBar)
        {
            searchBar.Text = string.Empty;
            searchBar.ResignFirstResponder();

            if (_owner.TryGetTarget(out DataScannerResultsViewController? controller))
            {
                controller.ApplyQuery(string.Empty);
            }
        }
    }

    internal sealed class ListItem : NSObject
    {
        public ListItem(string value, DateTime timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }

        public string Value { get; set; }

        public DateTime Timestamp { get; set; }

        internal bool HighlightOnAppear { get; set; }
    }

    private sealed class EditAlertViewController : NSObject
    {
        private readonly UIAlertController _alert;
        private readonly UIAlertAction _cancelAction;
        private readonly UIAlertAction _editAction;

        public EditAlertViewController(Action<UIAlertAction> save)
        {
            _alert = UIAlertController.Create("Edit", "Correct the scanned value.", UIAlertControllerStyle.Alert);
            _alert.AddTextField(field =>
            {
                field?.ClearButtonMode = UITextFieldViewMode.WhileEditing;
                field?.AutocorrectionType = UITextAutocorrectionType.No;
                field?.AutocapitalizationType = UITextAutocapitalizationType.AllCharacters;
                field?.ReturnKeyType = UIReturnKeyType.Done;
            });

            _cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null);
            _editAction = UIAlertAction.Create("Save", UIAlertActionStyle.Default, save);

            _alert.AddAction(_cancelAction);
            _alert.AddAction(_editAction);
        }

        public string Value
        {
            get
            {
                if (_alert.TextFields.FirstOrDefault()?.Text is not string value)
                {
                    return string.Empty;
                }

                return value;
            }

            set
            {
                if (_alert.TextFields.FirstOrDefault() is not UITextField textField)
                {
                    return;
                }

                textField.Text = value;
            }
        }

        public ListItem Item { get; set; }

        public UIAlertController Alert => _alert;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _cancelAction.Dispose();
                _editAction.Dispose();
                _alert.Dispose();
            }
        }
    }
}
