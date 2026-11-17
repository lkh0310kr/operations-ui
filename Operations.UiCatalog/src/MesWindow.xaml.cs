using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Operations.UiCatalog;

public partial class MesWindow : Window
{
    private readonly ICollectionView _orders;
    private readonly Order[] _allOrders;
    private string? _selectedId;
    private bool _refreshing;
    public MesWindow()
    {
        InitializeComponent();
        _allOrders = Enumerable.Range(1, 10000).Select(i => new Order(
            $"WO-{i:000000}", i % 2 == 0 ? "구동 모듈 A" : "제어 유닛 B",
            i % 3 == 0 ? "대기" : "진행 중", i % 800, "2026-11-17")).ToArray();
        _orders = CollectionViewSource.GetDefaultView(_allOrders);
        _orders.Filter = Filter;
        OrdersGrid.ItemsSource = _orders;
        OrdersGrid.SelectedIndex = 0;
    }
    private bool Filter(object item) => item is Order order &&
        (order.Id.Contains(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) || order.Product.Contains(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase)) &&
        (StateFilter.SelectedIndex <= 0 || order.State == (StateFilter.SelectedIndex == 1 ? "진행 중" : "대기"));
    private void SearchChanged(object sender, TextChangedEventArgs e) => RefreshView();
    private void FilterChanged(object sender, SelectionChangedEventArgs e) => RefreshView();
    private void RefreshClick(object sender, RoutedEventArgs e) => RefreshView();

    private void RefreshView()
    {
        if (_orders is null) return; // Initialization can raise change events before the view exists.
        _refreshing = true;
        try
        {
            _orders.Refresh();
            var selected = _allOrders.FirstOrDefault(order => order.Id == _selectedId);
            OrdersGrid.SelectedItem = selected is not null && _orders.Contains(selected) ? selected : null;
        }
        finally { _refreshing = false; }
    }
    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var order = OrdersGrid.SelectedItem as Order;
        if (!_refreshing) _selectedId = order?.Id;
        DetailId.Text = order?.Id ?? "선택된 작업 없음";
        DetailProduct.Text = order?.Product ?? "목록에서 작업을 선택하세요.";
        DetailState.Content = order?.State ?? "—";
        DetailQuantity.Text = order?.Quantity ?? "—";
        DetailDue.Text = order?.Due ?? "—";
    }

    internal string[] VerifySampleBehavior()
    {
        OrdersGrid.SelectedItem = _allOrders[2]; // WO-000003 is waiting.
        StateFilter.SelectedIndex = 1;
        if (OrdersGrid.SelectedItem is not null || (string)DetailState.Content != "—")
            throw new InvalidOperationException("Hidden selection must not remain actionable in the inspector.");
        StateFilter.SelectedIndex = 0;
        if (OrdersGrid.SelectedItem is not Order { Id: "WO-000003" } || (string)DetailState.Content != "대기")
            throw new InvalidOperationException("Selection was not restored by ID.");
        SearchBox.Text = "WO-000020";
        if (OrdersGrid.SelectedItem is not null || _orders.Cast<Order>().Single().Id != "WO-000020")
            throw new InvalidOperationException("Search did not filter the view.");
        SearchBox.Text = "";
        if (OrdersGrid.SelectedItem is not Order { Id: "WO-000003" })
            throw new InvalidOperationException("Search did not restore selection.");
        _orders.SortDescriptions.Add(new SortDescription(nameof(Order.Produced), ListSortDirection.Descending));
        if (_orders.Cast<Order>().First().Produced != 799)
            throw new InvalidOperationException("Numeric sort failed.");
        if (!OrdersGrid.EnableRowVirtualization || !OrdersGrid.EnableColumnVirtualization)
            throw new InvalidOperationException("UI virtualization is disabled.");
        return ["State filter hides and restores the selected ID", "Search filters and restores the selected ID",
                "Produced quantity sorts numerically", "Row and column UI virtualization enabled"];
    }
    private sealed record Order(string Id, string Product, string State, int Produced, string Due)
    {
        public string Quantity => $"{Produced:N0} / 800 EA";
    }
}
