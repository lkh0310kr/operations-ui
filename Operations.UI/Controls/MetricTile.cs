using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

public enum MetricTileState
{
    Value,
    Loading,
    Empty,
    Error
}

/// <summary>Displays one operational measurement and its supporting context.</summary>
public sealed class MetricTile : Control
{
    static MetricTile()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MetricTile), new FrameworkPropertyMetadata(typeof(MetricTile)));
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(MetricTile), new PropertyMetadata(string.Empty));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(string), typeof(MetricTile), new PropertyMetadata("—"));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
        nameof(Unit), typeof(string), typeof(MetricTile), new PropertyMetadata(string.Empty));

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(
        nameof(Context), typeof(string), typeof(MetricTile), new PropertyMetadata(string.Empty));

    public string Context
    {
        get => (string)GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(MetricTileState), typeof(MetricTile), new PropertyMetadata(MetricTileState.Value));

    public MetricTileState State
    {
        get => (MetricTileState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty StateMessageProperty = DependencyProperty.Register(
        nameof(StateMessage), typeof(string), typeof(MetricTile), new PropertyMetadata(string.Empty));

    public string StateMessage
    {
        get => (string)GetValue(StateMessageProperty);
        set => SetValue(StateMessageProperty, value);
    }
}
