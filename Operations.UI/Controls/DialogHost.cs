using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Operations.UI.Controls;

/// <summary>Displays modal content inside the current window without creating another native window.</summary>
public sealed class DialogHost : ContentControl
{
    private Button? _backdropButton;
    private IInputElement? _previousFocus;

    static DialogHost()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHost), new FrameworkPropertyMetadata(typeof(DialogHost)));
    }

    public static readonly DependencyProperty DialogContentProperty = DependencyProperty.Register(
        nameof(DialogContent), typeof(object), typeof(DialogHost), new PropertyMetadata(null));

    public object? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    public static readonly DependencyProperty IsDialogOpenProperty = DependencyProperty.Register(
        nameof(IsDialogOpen), typeof(bool), typeof(DialogHost), new FrameworkPropertyMetadata(false, OnIsDialogOpenChanged));

    public bool IsDialogOpen
    {
        get => (bool)GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    public static readonly DependencyProperty CanDismissOnBackdropProperty = DependencyProperty.Register(
        nameof(CanDismissOnBackdrop), typeof(bool), typeof(DialogHost), new PropertyMetadata(false));

    public bool CanDismissOnBackdrop
    {
        get => (bool)GetValue(CanDismissOnBackdropProperty);
        set => SetValue(CanDismissOnBackdropProperty, value);
    }

    public static readonly DependencyProperty DialogMaxWidthProperty = DependencyProperty.Register(
        nameof(DialogMaxWidth), typeof(double), typeof(DialogHost), new PropertyMetadata(720d));

    public double DialogMaxWidth
    {
        get => (double)GetValue(DialogMaxWidthProperty);
        set => SetValue(DialogMaxWidthProperty, value);
    }

    public static readonly DependencyProperty DialogMaxHeightProperty = DependencyProperty.Register(
        nameof(DialogMaxHeight), typeof(double), typeof(DialogHost), new PropertyMetadata(760d));

    public double DialogMaxHeight
    {
        get => (double)GetValue(DialogMaxHeightProperty);
        set => SetValue(DialogMaxHeightProperty, value);
    }

    public static readonly DependencyProperty CanDismissWithEscapeProperty = DependencyProperty.Register(
        nameof(CanDismissWithEscape), typeof(bool), typeof(DialogHost), new PropertyMetadata(true));

    public bool CanDismissWithEscape
    {
        get => (bool)GetValue(CanDismissWithEscapeProperty);
        set => SetValue(CanDismissWithEscapeProperty, value);
    }

    public static readonly DependencyProperty DialogPaddingProperty = DependencyProperty.Register(
        nameof(DialogPadding), typeof(Thickness), typeof(DialogHost), new PropertyMetadata(new Thickness(24)));

    public Thickness DialogPadding
    {
        get => (Thickness)GetValue(DialogPaddingProperty);
        set => SetValue(DialogPaddingProperty, value);
    }

    public static readonly RoutedEvent DismissRequestedEvent = EventManager.RegisterRoutedEvent(
        nameof(DismissRequested), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DialogHost));

    public event RoutedEventHandler DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    public override void OnApplyTemplate()
    {
        if (_backdropButton is not null) _backdropButton.Click -= OnBackdropClick;
        PreviewKeyDown -= OnPreviewKeyDown;
        base.OnApplyTemplate();
        _backdropButton = GetTemplateChild("PART_Backdrop") as Button;
        if (_backdropButton is not null) _backdropButton.Click += OnBackdropClick;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private void OnBackdropClick(object sender, RoutedEventArgs e)
    {
        if (!CanDismissOnBackdrop) return;
        SetCurrentValue(IsDialogOpenProperty, false);
        RaiseEvent(new RoutedEventArgs(DismissRequestedEvent, this));
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!IsDialogOpen || !CanDismissWithEscape || e.Key != Key.Escape) return;
        SetCurrentValue(IsDialogOpenProperty, false);
        RaiseEvent(new RoutedEventArgs(DismissRequestedEvent, this));
        e.Handled = true;
    }

    private static void OnIsDialogOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DialogHost host) return;
        if (e.NewValue is true)
        {
            host._previousFocus = Keyboard.FocusedElement;
            host.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => Keyboard.Focus(host.GetTemplateChild("PART_DialogContent") as IInputElement ?? host)));
            return;
        }

        var previousFocus = host._previousFocus;
        host._previousFocus = null;
        if (previousFocus is not null)
            host.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => Keyboard.Focus(previousFocus)));
    }
}
