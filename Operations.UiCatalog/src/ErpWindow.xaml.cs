using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Operations.UiCatalog;

public partial class ErpWindow : Window
{
    public ErpWindow()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel();
    }

    internal async Task<string[]> VerifySampleBehaviorAsync()
    {
        var model = (InventoryViewModel)DataContext;
        var item = model.Selected!;
        var original = item.Reorder;
        if (!CodeInput.IsReadOnly || !CodeInput.IsEnabled) throw new InvalidOperationException("Code must be read-only and enabled.");
        NameInput.Text = " ";
        QuantityInput.Text = "-1";
        await model.SaveAsync();
        UpdateLayout();
        if (!NameField.IsInvalid || !QuantityField.IsInvalid || item.Reorder != original)
            throw new InvalidOperationException("Validation changed applied data or did not report errors.");
        NameInput.Text = "검증 샘플";
        QuantityInput.Text = "250";
        model.Selected = model.Items[1];
        model.Selected = item;
        if (model.DraftName != "검증 샘플" || model.DraftReorder != "250")
            throw new InvalidOperationException("Switching items discarded the draft.");
        model.SimulateFailure = true;
        var failed = model.SaveAsync();
        UpdateLayout();
        if (!model.IsBusy || model.CanEdit || !SaveButton.IsBusy || !NameInput.IsReadOnly) throw new InvalidOperationException("Save did not enter busy state.");
        await failed;
        if (item.Reorder != original || !model.SaveMessage.Contains("실패"))
            throw new InvalidOperationException("Failure changed applied data or was not reported.");
        model.SimulateFailure = false;
        await model.SaveAsync();
        if (item.Reorder != 250 || item.Name != "검증 샘플" || !model.SaveMessage.Contains("완료"))
            throw new InvalidOperationException("Successful sample save was not applied.");
        return ["Read-only code remains enabled", "Input bindings report required and range validation without changing applied data", "Draft is retained when switching items", "Busy locks editing and updates the command button", "Simulated failure preserves applied data", "Simulated success applies the validated draft in memory"];
    }
}

internal sealed class InventoryItem(string code, string name, int quantity, decimal unitPrice, int reorder) : INotifyPropertyChanged
{
    private string _name = name;
    private int _reorder = reorder;
    public string Code { get; } = code;
    public string Name { get => _name; set { _name = value; Changed(); } }
    public int Quantity { get; } = quantity;
    public decimal UnitPrice { get; } = unitPrice;
    public decimal Amount => Quantity * UnitPrice;
    public int Reorder { get => _reorder; set { _reorder = value; Changed(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

internal sealed class InventoryViewModel : INotifyPropertyChanged
{
    private InventoryItem? _selected;
    private string _draftName = "", _draftReorder = "", _saveMessage = "변경 사항을 입력한 뒤 저장하세요.";
    private bool _isBusy, _nameInvalid, _quantityInvalid, _simulateFailure;
    private readonly AsyncSampleCommand _saveCommand;
    private readonly Dictionary<string, (string Name, string Reorder)> _drafts = new();

    public ObservableCollection<InventoryItem> Items { get; } =
    [
        new("000123", "서보 모터 2 kW", 48, 485000m, 20),
        new("000124", "온도 센서 PT100", 132, 42600m, 50),
        new("000215", "제어 밸브 DN25", 17, 318000m, 10),
        new("000302", "산업용 케이블 5 m", 350, 18300m, 100),
        new("001008", "안전 릴레이 모듈", 8, 195000m, 12)
    ];
    public InventoryItem? Selected
    {
        get => _selected;
        set
        {
            if (_isBusy || ReferenceEquals(_selected, value)) return;
            if (_selected is not null) _drafts[_selected.Code] = (DraftName, DraftReorder);
            _selected = value;
            var draft = value is not null && _drafts.TryGetValue(value.Code, out var saved) ? saved :
                (value?.Name ?? "", value?.Reorder.ToString(CultureInfo.CurrentCulture) ?? "");
            DraftName = draft.Item1;
            DraftReorder = draft.Item2;
            NameInvalid = QuantityInvalid = false;
            SaveMessage = value is null ? "품목을 선택하세요." : "변경 사항을 입력한 뒤 저장하세요.";
            Changed();
            _saveCommand.NotifyCanExecuteChanged();
        }
    }
    public string DraftName { get => _draftName; set { _draftName = value; Changed(); } }
    public string DraftReorder { get => _draftReorder; set { _draftReorder = value; Changed(); } }
    public bool SimulateFailure { get => _simulateFailure; set { _simulateFailure = value; Changed(); } }
    public bool NameInvalid { get => _nameInvalid; private set { _nameInvalid = value; Changed(); } }
    public bool QuantityInvalid { get => _quantityInvalid; private set { _quantityInvalid = value; Changed(); } }
    public string NameError => "품목명은 1–80자로 입력해야 합니다.";
    public string QuantityError => "0–100,000 사이의 정수를 입력해야 합니다.";
    public bool IsBusy { get => _isBusy; private set { _isBusy = value; Changed(); Changed(nameof(CanEdit)); _saveCommand.NotifyCanExecuteChanged(); } }
    public bool CanEdit => !IsBusy;
    public string SaveMessage { get => _saveMessage; private set { _saveMessage = value; Changed(); } }
    public ICommand SaveCommand => _saveCommand;
    public event PropertyChangedEventHandler? PropertyChanged;

    public InventoryViewModel()
    {
        _saveCommand = new AsyncSampleCommand(SaveAsync, () => Selected is not null && !IsBusy);
        Selected = Items[0];
    }

    public async Task SaveAsync()
    {
        if (IsBusy || Selected is null) return;
        var name = DraftName.Trim();
        NameInvalid = name.Length is < 1 or > 80;
        QuantityInvalid = !int.TryParse(DraftReorder, NumberStyles.Integer, CultureInfo.CurrentCulture, out var reorder) || reorder is < 0 or > 100000;
        if (NameInvalid || QuantityInvalid) { SaveMessage = "입력 오류를 수정하세요. 적용된 값은 변경되지 않았습니다."; return; }
        var target = Selected;
        var fail = SimulateFailure;
        IsBusy = true;
        SaveMessage = "샘플 저장 중…";
        try
        {
            await Task.Delay(400); // A simulated delay, not an external request.
            if (fail) { SaveMessage = "샘플 저장 실패 · 적용된 값은 변경되지 않았습니다."; return; }
            target.Name = name;
            target.Reorder = reorder;
            SaveMessage = "샘플 저장 완료 · 이 창의 메모리에 적용되었습니다.";
        }
        finally { IsBusy = false; }
    }
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

internal sealed class AsyncSampleCommand(Func<Task> execute, Func<bool> canExecute) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => canExecute();
    public async void Execute(object? parameter) { if (CanExecute(parameter)) await execute(); }
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
