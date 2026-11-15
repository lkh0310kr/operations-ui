using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

public enum DataStateKind
{
    Loading,
    Empty,
    Error
}

/// <summary>Shows a consistent loading, empty, or error state for a data region.</summary>
public sealed class DataStatePanel : Control
{
    static DataStatePanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DataStatePanel), new FrameworkPropertyMetadata(typeof(DataStatePanel)));
    }

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(DataStateKind), typeof(DataStatePanel), new PropertyMetadata(DataStateKind.Empty));

    public DataStateKind State
    {
        get => (DataStateKind)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(DataStatePanel), new PropertyMetadata("표시할 결과가 없습니다."));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message), typeof(string), typeof(DataStatePanel), new PropertyMetadata(string.Empty));

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty ActionContentProperty = DependencyProperty.Register(
        nameof(ActionContent), typeof(object), typeof(DataStatePanel), new PropertyMetadata(null));

    /// <summary>Optional action control, such as a retry button. The supplied control owns its own click behavior.</summary>
    public object? ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }
}
