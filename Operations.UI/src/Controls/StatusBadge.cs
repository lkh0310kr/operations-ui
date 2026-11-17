using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

public enum EquipmentState
{
    Offline,
    Ready,
    Running,
    Warning,
    Fault
}

/// <summary>Legacy compatibility control. New operational screens should use separate status indicators for each independent fact.</summary>
public sealed class StatusBadge : ContentControl
{
    static StatusBadge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBadge), new FrameworkPropertyMetadata(typeof(StatusBadge)));
    }

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(EquipmentState), typeof(StatusBadge), new PropertyMetadata(EquipmentState.Offline));

    public EquipmentState State
    {
        get => (EquipmentState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
}
