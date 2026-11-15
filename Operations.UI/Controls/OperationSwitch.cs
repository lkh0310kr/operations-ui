using System.Windows;
using System.Windows.Controls.Primitives;

namespace Operations.UI.Controls;

/// <summary>A keyboard-accessible on/off switch using the standard ToggleButton checked state.</summary>
public sealed class OperationSwitch : ToggleButton
{
    static OperationSwitch()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(OperationSwitch),
            new FrameworkPropertyMetadata(typeof(OperationSwitch)));
    }
}
