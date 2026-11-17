using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Operations.UI.Controls;

/// <summary>Displays a caller-formatted value and independent source quality, freshness and time descriptions.</summary>
public sealed class ValueDisplay : Control
{
    static ValueDisplay() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ValueDisplay), new FrameworkPropertyMetadata(typeof(ValueDisplay)));
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(nameof(ValueText), typeof(string), typeof(ValueDisplay), new PropertyMetadata(null));
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public static readonly DependencyProperty RangeTextProperty = DependencyProperty.Register(nameof(RangeText), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public static readonly DependencyProperty QualityTextProperty = DependencyProperty.Register(nameof(QualityText), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public static readonly DependencyProperty FreshnessTextProperty = DependencyProperty.Register(nameof(FreshnessText), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public static readonly DependencyProperty TimestampTextProperty = DependencyProperty.Register(nameof(TimestampText), typeof(string), typeof(ValueDisplay), new PropertyMetadata(""));
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string? ValueText { get => (string?)GetValue(ValueTextProperty); set => SetValue(ValueTextProperty, value); }
    public string Unit { get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value); }
    public string RangeText { get => (string)GetValue(RangeTextProperty); set => SetValue(RangeTextProperty, value); }
    public string QualityText { get => (string)GetValue(QualityTextProperty); set => SetValue(QualityTextProperty, value); }
    public string FreshnessText { get => (string)GetValue(FreshnessTextProperty); set => SetValue(FreshnessTextProperty, value); }
    public string TimestampText { get => (string)GetValue(TimestampTextProperty); set => SetValue(TimestampTextProperty, value); }
    protected override AutomationPeer OnCreateAutomationPeer() => new ValueDisplayAutomationPeer(this);
}

internal sealed class ValueDisplayAutomationPeer(ValueDisplay owner) : FrameworkElementAutomationPeer(owner)
{
    protected override string GetClassNameCore() => nameof(ValueDisplay);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;
    protected override string GetNameCore()
    {
        var explicitName = base.GetNameCore();
        return !string.IsNullOrWhiteSpace(explicitName) ? explicitName :
            string.Join(", ", new[] { owner.Label, owner.ValueText ?? "—", owner.Unit, owner.RangeText, owner.QualityText, owner.FreshnessText, owner.TimestampText }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
