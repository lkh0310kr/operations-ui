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

public enum OperationButtonSize
{
    Compact,
    Standard,
    Large
}

/// <summary>A standard WPF button with Operations UI sizing and semantic variants.</summary>
public sealed class OperationButton : System.Windows.Controls.Button
{
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

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        nameof(Size), typeof(OperationButtonSize), typeof(OperationButton),
        new FrameworkPropertyMetadata(OperationButtonSize.Standard));

    /// <summary>Sets a consistent interaction size: compact (36), standard (44), or large (52) device independent pixels.</summary>
    public OperationButtonSize Size
    {
        get => (OperationButtonSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy), typeof(bool), typeof(OperationButton), new FrameworkPropertyMetadata(false));

    /// <summary>Shows progress and prevents repeat activation while an operation is in progress.</summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register(
        nameof(BusyContent), typeof(object), typeof(OperationButton), new FrameworkPropertyMetadata("처리 중…"));

    public object? BusyContent
    {
        get => GetValue(BusyContentProperty);
        set => SetValue(BusyContentProperty, value);
    }

    protected override void OnClick()
    {
        if (!IsBusy) base.OnClick();
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
