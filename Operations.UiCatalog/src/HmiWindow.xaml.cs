using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Operations.UI.Controls;

namespace Operations.UiCatalog;

public partial class HmiWindow : Window
{
    public HmiWindow()
    {
        InitializeComponent();
        DataContext = new HmiSampleViewModel();
    }

    internal string[] VerifySampleBehavior()
    {
        var model = (HmiSampleViewModel)DataContext;
        if (RequestButton.IsEnabled || RequestButton.MinHeight < 48 || TemperatureTrend.Samples?.Count != 11)
            throw new InvalidOperationException("HMI sample resources or data are invalid.");
        if (model.Samples[5].Value is not null || model.Samples[5].Quality != SampleQuality.Bad)
            throw new InvalidOperationException("Missing trend sample was lost.");
        model.AcknowledgeCommand.Execute(null);
        if (!model.IsAcknowledged || !model.IsAlarmActive)
            throw new InvalidOperationException("Acknowledgement must not clear the active alarm.");
        return ["Touch minimum height and disabled command verified", "Missing trend sample retained", "Acknowledgement and alarm active state remain independent"];
    }
}

internal sealed class HmiSampleViewModel : INotifyPropertyChanged
{
    private bool _acknowledged;
    public DateTimeOffset Start { get; } = new(2026, 11, 17, 10, 14, 0, TimeSpan.FromHours(9));
    public DateTimeOffset End => Start.AddMinutes(10);
    public IReadOnlyList<TrendSample> Samples { get; }
    public bool IsAlarmActive => true;
    public bool IsAcknowledged => _acknowledged;
    public string AlarmActiveDescription => IsAlarmActive ? "상태: 활성" : "상태: 해소";
    public string Acknowledgement => _acknowledged ? "확인: 확인됨 (샘플)" : "확인: 미확인";
    public ICommand AcknowledgeCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public HmiSampleViewModel()
    {
        double?[] values = [68.1, 69.2, 70.3, 71.6, 72.0, null, 72.7, 73.1, 73.0, 73.2, 73.4];
        Samples = values.Select((v, i) => new TrendSample(Start.AddMinutes(i), v, v is null ? SampleQuality.Bad : SampleQuality.Good)).ToArray();
        AcknowledgeCommand = new SampleCommand(() =>
        {
            _acknowledged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Acknowledgement)));
        });
    }
}

internal sealed class SampleCommand(Action execute) : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => execute();
}
