using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Operations.UiCatalog;

public partial class MesWindow : Window
{
    private readonly ICollectionView _orders;
    public MesWindow()
    {
        InitializeComponent();
        var items = Enumerable.Range(1, 10000).Select(i => new Order(
            $"WO-{i:000000}", i % 2 == 0 ? "구동 모듈 A" : "제어 유닛 B",
            i % 3 == 0 ? "대기" : "진행 중", $"{i % 800:N0} / 800 EA", "2026-11-17")).ToArray();
        _orders = CollectionViewSource.GetDefaultView(items);
        _orders.Filter = Filter;
        OrdersGrid.ItemsSource = _orders;
        OrdersGrid.SelectedIndex = 0;
    }
    private bool Filter(object item) => item is Order order &&
        (order.Id.Contains(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase) || order.Product.Contains(SearchBox.Text, StringComparison.CurrentCultureIgnoreCase));
    private void SearchChanged(object sender, TextChangedEventArgs e) { if (_orders is not null) _orders.Refresh(); }
    private void RefreshClick(object sender, RoutedEventArgs e) => _orders.Refresh();
    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var order = OrdersGrid.SelectedItem as Order;
        DetailId.Text = order?.Id ?? "선택된 작업 없음";
        DetailProduct.Text = order?.Product ?? "목록에서 작업을 선택하세요.";
        DetailState.Text = order?.State ?? "—";
        DetailQuantity.Text = order?.Quantity ?? "—";
        DetailDue.Text = order?.Due ?? "—";
    }
    private sealed record Order(string Id, string Product, string State, string Quantity, string Due);
}
