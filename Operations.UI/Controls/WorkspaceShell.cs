using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

/// <summary>A reusable desktop work area with optional navigation, header, content, and status regions.</summary>
public sealed class WorkspaceShell : ContentControl
{
    static WorkspaceShell()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WorkspaceShell), new FrameworkPropertyMetadata(typeof(WorkspaceShell)));
    }

    public static readonly DependencyProperty NavigationContentProperty = DependencyProperty.Register(
        nameof(NavigationContent), typeof(object), typeof(WorkspaceShell), new PropertyMetadata(null));

    public object? NavigationContent
    {
        get => GetValue(NavigationContentProperty);
        set => SetValue(NavigationContentProperty, value);
    }

    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
        nameof(HeaderContent), typeof(object), typeof(WorkspaceShell), new PropertyMetadata(null));

    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public static readonly DependencyProperty StatusContentProperty = DependencyProperty.Register(
        nameof(StatusContent), typeof(object), typeof(WorkspaceShell), new PropertyMetadata(null));

    public object? StatusContent
    {
        get => GetValue(StatusContentProperty);
        set => SetValue(StatusContentProperty, value);
    }

    public static readonly DependencyProperty NavigationWidthProperty = DependencyProperty.Register(
        nameof(NavigationWidth), typeof(GridLength), typeof(WorkspaceShell), new PropertyMetadata(new GridLength(224)));

    public GridLength NavigationWidth
    {
        get => (GridLength)GetValue(NavigationWidthProperty);
        set => SetValue(NavigationWidthProperty, value);
    }

    public static readonly DependencyProperty ShowNavigationProperty = DependencyProperty.Register(
        nameof(ShowNavigation), typeof(bool), typeof(WorkspaceShell), new PropertyMetadata(true));

    public bool ShowNavigation
    {
        get => (bool)GetValue(ShowNavigationProperty);
        set => SetValue(ShowNavigationProperty, value);
    }

    public static readonly DependencyProperty ShowStatusBarProperty = DependencyProperty.Register(
        nameof(ShowStatusBar), typeof(bool), typeof(WorkspaceShell), new PropertyMetadata(true));

    public bool ShowStatusBar
    {
        get => (bool)GetValue(ShowStatusBarProperty);
        set => SetValue(ShowStatusBarProperty, value);
    }
}
