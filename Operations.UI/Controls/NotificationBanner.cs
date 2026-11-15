using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

public enum NotificationSeverity
{
    Information,
    Success,
    Warning,
    Error
}

/// <summary>A compact, accessible message surface for non-blocking application feedback.</summary>
public sealed class NotificationBanner : ContentControl
{
    private Button? _dismissButton;

    static NotificationBanner()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBanner), new FrameworkPropertyMetadata(typeof(NotificationBanner)));
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(NotificationBanner), new PropertyMetadata(string.Empty));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message), typeof(string), typeof(NotificationBanner), new PropertyMetadata(string.Empty));

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity), typeof(NotificationSeverity), typeof(NotificationBanner), new PropertyMetadata(NotificationSeverity.Information));

    public NotificationSeverity Severity
    {
        get => (NotificationSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public static readonly DependencyProperty IsDismissibleProperty = DependencyProperty.Register(
        nameof(IsDismissible), typeof(bool), typeof(NotificationBanner), new PropertyMetadata(true));

    public bool IsDismissible
    {
        get => (bool)GetValue(IsDismissibleProperty);
        set => SetValue(IsDismissibleProperty, value);
    }

    public static readonly RoutedEvent DismissRequestedEvent = EventManager.RegisterRoutedEvent(
        nameof(DismissRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationBanner));

    public event RoutedEventHandler DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    public override void OnApplyTemplate()
    {
        if (_dismissButton is not null) _dismissButton.Click -= OnDismissClick;
        base.OnApplyTemplate();
        _dismissButton = GetTemplateChild("PART_DismissButton") as Button;
        if (_dismissButton is not null) _dismissButton.Click += OnDismissClick;
    }

    private void OnDismissClick(object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(DismissRequestedEvent, this));
}
