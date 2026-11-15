using System.Windows;
using Wpf.Ui.Controls;

namespace Operations.UI.Controls;

public enum OperationButtonVariant
{
    Standard,
    Primary,
    Success,
    Warning,
    Danger
}

/// <summary>A WPF UI button with Operations UI sizing and a small semantic variant API.</summary>
public sealed class OperationButton : Wpf.Ui.Controls.Button
{
    public OperationButton()
    {
        MinHeight = 44;
        Padding = new Thickness(16, 9, 16, 9);
        CornerRadius = new CornerRadius(6);
        FontWeight = FontWeights.SemiBold;
    }

    static OperationButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(OperationButton),
            new FrameworkPropertyMetadata(typeof(OperationButton)));
    }

    public static readonly DependencyProperty VariantProperty = DependencyProperty.Register(
        nameof(Variant), typeof(OperationButtonVariant), typeof(OperationButton),
        new FrameworkPropertyMetadata(OperationButtonVariant.Standard, OnVariantChanged));

    public OperationButtonVariant Variant
    {
        get => (OperationButtonVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private static void OnVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not OperationButton button) return;

        button.Appearance = (OperationButtonVariant)e.NewValue switch
        {
            OperationButtonVariant.Primary => ControlAppearance.Primary,
            OperationButtonVariant.Success => ControlAppearance.Success,
            OperationButtonVariant.Warning => ControlAppearance.Caution,
            OperationButtonVariant.Danger => ControlAppearance.Danger,
            _ => ControlAppearance.Secondary
        };
    }
}
