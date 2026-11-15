using System.Windows;

namespace Operations.UI.Controls;

public enum OperationButtonVariant
{
    Standard,
    Primary,
    Success,
    Warning,
    Danger
}

/// <summary>A standard WPF button with Operations UI sizing and semantic variants.</summary>
public sealed class OperationButton : System.Windows.Controls.Button
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
        new FrameworkPropertyMetadata(OperationButtonVariant.Standard));

    public OperationButtonVariant Variant
    {
        get => (OperationButtonVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius), typeof(CornerRadius), typeof(OperationButton),
        new FrameworkPropertyMetadata(new CornerRadius(6)));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
