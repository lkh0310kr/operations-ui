using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Operations.UI.Controls;

/// <summary>Pairs a control with its label, help, required, and validation messages.</summary>
public sealed class FormField : ContentControl
{
    private TextBlock? _labelPart;

    static FormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FormField),
            new FrameworkPropertyMetadata(typeof(FormField)));
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(FormField),
        new FrameworkPropertyMetadata(string.Empty, OnFieldMetadataChanged));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register(
        nameof(HelpText), typeof(string), typeof(FormField),
        new FrameworkPropertyMetadata(string.Empty, OnFieldMetadataChanged));

    public string HelpText
    {
        get => (string)GetValue(HelpTextProperty);
        set => SetValue(HelpTextProperty, value);
    }

    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
        nameof(ErrorMessage), typeof(string), typeof(FormField),
        new FrameworkPropertyMetadata(string.Empty, OnFieldMetadataChanged));

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public static readonly DependencyProperty IsRequiredProperty = DependencyProperty.Register(
        nameof(IsRequired), typeof(bool), typeof(FormField), new PropertyMetadata(false, OnFieldMetadataChanged));

    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register(
        nameof(IsInvalid), typeof(bool), typeof(FormField),
        new FrameworkPropertyMetadata(false, OnFieldMetadataChanged));

    public bool IsInvalid
    {
        get => (bool)GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _labelPart = GetTemplateChild("PART_Label") as TextBlock;
        UpdateAutomationMetadata();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        UpdateAutomationMetadata();
    }

    private static void OnFieldMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FormField field)
            field.UpdateAutomationMetadata();
    }

    private void UpdateAutomationMetadata()
    {
        if (Content is not UIElement input)
            return;

        if (_labelPart is not null)
            AutomationProperties.SetLabeledBy(input, _labelPart);

        AutomationProperties.SetIsRequiredForForm(input, IsRequired);

        var accessibleHelp = IsInvalid && !string.IsNullOrWhiteSpace(ErrorMessage)
            ? ErrorMessage
            : HelpText;
        AutomationProperties.SetHelpText(input, accessibleHelp ?? string.Empty);
    }
}
