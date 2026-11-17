using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
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
        var valueName = UIElementAutomationPeer.CreatePeerForElement(SpeedValue)?.GetName() ?? "";
        if (SpeedValue.ValueText is not null || !valueName.Contains("—") || !valueName.Contains("품질") || !valueName.Contains("최신성"))
            throw new InvalidOperationException("Missing value, quality and freshness are not separately exposed.");
        var alarmPeer = UIElementAutomationPeer.CreatePeerForElement(SampleAlarm);
        var alarmName = alarmPeer?.GetName() ?? "";
        if (!alarmName.Contains("활성") || !alarmName.Contains("미확인"))
            throw new InvalidOperationException("Alarm active and acknowledgement descriptions are not exposed.");
        if (SampleAlarm.Template.FindName("AlarmTitle", SampleAlarm) is not System.Windows.Controls.TextBlock title || title.Text != SampleAlarm.Title)
            throw new InvalidOperationException("Alarm title is not visible in the presentation template.");
        if (SampleAlarm.Template.FindName("AcknowledgeButton", SampleAlarm) is not OperationButton button || button.Command != model.AcknowledgeCommand)
            throw new InvalidOperationException("Alarm acknowledgement command is not wired to the supplied command.");
        button.Command.Execute(button.CommandParameter);
        if (!model.IsAcknowledged || !model.IsAlarmActive || SampleAlarm.AcknowledgementText != model.Acknowledgement || SampleAlarm.ActiveText != model.AlarmActiveDescription)
            throw new InvalidOperationException("Acknowledgement must not clear the active alarm.");
        if (alarmPeer?.GetName().Contains("확인됨") != true)
            throw new InvalidOperationException("Updated alarm acknowledgement is not available through the automation peer.");
        return ["Touch minimum height and disabled command verified", "Missing trend sample retained", "Missing value, quality and freshness exposed through automation peer", "Alarm active and acknowledgement descriptions are separate", "Acknowledgement and alarm active state remain independent"];
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
