using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace Operations.UI.Controls;

/// <summary>Presentation only: the application owns severity, activation, acknowledgement and command outcomes.</summary>
public sealed class AlarmEntry : Control
{
    static AlarmEntry() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AlarmEntry), new FrameworkPropertyMetadata(typeof(AlarmEntry)));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty IdentifierTextProperty = DependencyProperty.Register(nameof(IdentifierText), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty SeverityTextProperty = DependencyProperty.Register(nameof(SeverityText), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty ActiveTextProperty = DependencyProperty.Register(nameof(ActiveText), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty AcknowledgementTextProperty = DependencyProperty.Register(nameof(AcknowledgementText), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty AcknowledgeLabelProperty = DependencyProperty.Register(nameof(AcknowledgeLabel), typeof(string), typeof(AlarmEntry), new PropertyMetadata("Acknowledge"));
    public static readonly DependencyProperty AcknowledgeHelpProperty = DependencyProperty.Register(nameof(AcknowledgeHelp), typeof(string), typeof(AlarmEntry), new PropertyMetadata(""));
    public static readonly DependencyProperty ToneProperty = DependencyProperty.Register(nameof(Tone), typeof(StatusTone), typeof(AlarmEntry), new PropertyMetadata(StatusTone.Neutral));
    public static readonly DependencyProperty AcknowledgeCommandProperty = DependencyProperty.Register(nameof(AcknowledgeCommand), typeof(ICommand), typeof(AlarmEntry), new PropertyMetadata(null));
    public static readonly DependencyProperty AcknowledgeCommandParameterProperty = DependencyProperty.Register(nameof(AcknowledgeCommandParameter), typeof(object), typeof(AlarmEntry), new PropertyMetadata(null));
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string IdentifierText { get => (string)GetValue(IdentifierTextProperty); set => SetValue(IdentifierTextProperty, value); }
    public string SeverityText { get => (string)GetValue(SeverityTextProperty); set => SetValue(SeverityTextProperty, value); }
    public string ActiveText { get => (string)GetValue(ActiveTextProperty); set => SetValue(ActiveTextProperty, value); }
    public string AcknowledgementText { get => (string)GetValue(AcknowledgementTextProperty); set => SetValue(AcknowledgementTextProperty, value); }
    public string AcknowledgeLabel { get => (string)GetValue(AcknowledgeLabelProperty); set => SetValue(AcknowledgeLabelProperty, value); }
    public string AcknowledgeHelp { get => (string)GetValue(AcknowledgeHelpProperty); set => SetValue(AcknowledgeHelpProperty, value); }
    public StatusTone Tone { get => (StatusTone)GetValue(ToneProperty); set => SetValue(ToneProperty, value); }
    public ICommand? AcknowledgeCommand { get => (ICommand?)GetValue(AcknowledgeCommandProperty); set => SetValue(AcknowledgeCommandProperty, value); }
    public object? AcknowledgeCommandParameter { get => GetValue(AcknowledgeCommandParameterProperty); set => SetValue(AcknowledgeCommandParameterProperty, value); }
    protected override AutomationPeer OnCreateAutomationPeer() => new AlarmEntryAutomationPeer(this);
}

internal sealed class AlarmEntryAutomationPeer(AlarmEntry owner) : FrameworkElementAutomationPeer(owner)
{
    protected override string GetClassNameCore() => nameof(AlarmEntry);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;
    protected override string GetNameCore()
    {
        var explicitName = base.GetNameCore();
        return !string.IsNullOrWhiteSpace(explicitName) ? explicitName :
            string.Join(", ", new[] { owner.Title, owner.IdentifierText, owner.SeverityText, owner.ActiveText, owner.AcknowledgementText }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}

/// <summary>Hosts alarm presentation items; lifecycle and ordering are supplied by the consumer.</summary>
public sealed class AlarmList : ItemsControl
{
    static AlarmList() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AlarmList), new FrameworkPropertyMetadata(typeof(AlarmList)));
}
