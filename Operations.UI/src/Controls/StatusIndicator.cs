using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

/// <summary>Visual emphasis only. The caller supplies the domain-specific, readable status text.</summary>
public enum StatusTone { Neutral, Positive, Warning, Danger }

/// <summary>Displays a short status as text and a supplemental marker.</summary>
public sealed class StatusIndicator : ContentControl
{
    static StatusIndicator() => DefaultStyleKeyProperty.OverrideMetadata(
        typeof(StatusIndicator), new FrameworkPropertyMetadata(typeof(StatusIndicator)));

    public static readonly DependencyProperty ToneProperty = DependencyProperty.Register(
        nameof(Tone), typeof(StatusTone), typeof(StatusIndicator), new PropertyMetadata(StatusTone.Neutral));

    public StatusTone Tone
    {
        get => (StatusTone)GetValue(ToneProperty);
        set => SetValue(ToneProperty, value);
    }
}
