using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Operations.UI.Controls;

namespace Operations.UiCatalog;

public partial class MainWindow
{
    private async Task<string[]> VerifyDialogAsync()
    {
        var report = new List<string>();
        var opener = new Button { Content = "Open" };
        var cancel = new Button { Content = "Cancel" };
        var confirm = new Button { Content = "_Confirm", Style = (Style)FindResource("PrimaryButton") };
        var actions = new StackPanel();
        actions.Children.Add(cancel);
        actions.Children.Add(confirm);
        var host = new DialogHost { Content = opener, DialogContent = actions, InitialFocus = cancel };
        var window = new Window { Owner = this, Width = 500, Height = 300, Content = host, Title = "Dialog verification" };
        var dismissed = 0;
        host.DismissRequested += (_, _) => dismissed++;
        window.Show();
        await Dispatcher.InvokeAsync(window.UpdateLayout, DispatcherPriority.Render);
        opener.Focus();
        Keyboard.Focus(opener);
        var canObserveFocus = Keyboard.FocusedElement == opener;
        host.IsDialogOpen = true;
        await Dispatcher.InvokeAsync(window.UpdateLayout, DispatcherPriority.ApplicationIdle);
        var background = host.Template.FindName("BackgroundContent", host) as ContentPresenter;
        var surface = host.Template.FindName("DialogSurface", host) as FrameworkElement;
        if (background is null || background.IsEnabled || surface is null || KeyboardNavigation.GetTabNavigation(surface) != KeyboardNavigationMode.Cycle)
            throw new InvalidOperationException("Dialog background or tab containment contract failed.");
        report.Add("Background disabled and dialog tab navigation cycles");
        if (canObserveFocus)
        {
            if (Keyboard.FocusedElement != cancel) throw new InvalidOperationException("Explicit initial focus was not applied.");
            report.Add("Explicit initial focus applied");
            if (!cancel.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)) || Keyboard.FocusedElement != confirm ||
                !confirm.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)) || Keyboard.FocusedElement != cancel ||
                !cancel.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous)) || Keyboard.FocusedElement != confirm)
                throw new InvalidOperationException("Forward or reverse focus traversal did not cycle inside the dialog.");
            report.Add("Programmatic forward and reverse focus traversal cycles within dialog");
        }
        else report.Add("SKIP: foreground keyboard focus unavailable; initial focus and restoration not observed");
        ApplyTheme(true);
        await Dispatcher.InvokeAsync(window.UpdateLayout, DispatcherPriority.Render);
        if (!host.IsDialogOpen || surface is not Border { Background: System.Windows.Media.SolidColorBrush brush } ||
            brush.Color != System.Windows.Media.Color.FromRgb(0xFF, 0xFF, 0xFF))
            throw new InvalidOperationException("Open dialog did not update to Light theme.");
        var accessText = FindVisual<AccessText>(confirm);
        if (accessText is null || !Equals(System.Windows.Documents.TextElement.GetForeground(accessText), confirm.Foreground))
            throw new InvalidOperationException("Primary button content does not use its foreground or preserve access text.");
        report.Add("Open dialog remains open and updates to Light theme");
        report.Add("Primary button uses its foreground and an AccessText element");
        var source = PresentationSource.FromVisual(window) ?? throw new InvalidOperationException("Dialog has no presentation source.");
        cancel.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, source, 0, Key.Escape) { RoutedEvent = Keyboard.PreviewKeyDownEvent });
        await Dispatcher.InvokeAsync(window.UpdateLayout, DispatcherPriority.ApplicationIdle);
        if (host.IsDialogOpen || dismissed != 1 || !background.IsEnabled)
            throw new InvalidOperationException("Escape dismissal did not restore dialog background.");
        if (canObserveFocus && Keyboard.FocusedElement != opener)
            throw new InvalidOperationException("Focus was not restored to opener.");
        report.Add("Escape requests dismissal once and re-enables background");
        if (canObserveFocus) report.Add("Focus restored to opener");
        host.IsDialogOpen = true;
        host.IsDialogOpen = false;
        await Dispatcher.InvokeAsync(window.UpdateLayout, DispatcherPriority.ApplicationIdle);
        if (host.IsDialogOpen || Keyboard.FocusedElement == cancel)
            throw new InvalidOperationException("A stale queued focus reopened or focused a closed dialog.");
        report.Add("Rapid open/close does not execute stale initial focus");
        window.Close();
        ApplyTheme(false);
        SelectCatalogSection(8);
        await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.ApplicationIdle);
        if (DialogPreviewHost.InitialFocus != DialogCancelButton)
            throw new InvalidOperationException("Catalog dialog initial focus binding is not resolved.");
        OpenPreviewDialog_Click(OpenDialogButton, new RoutedEventArgs());
        await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.ApplicationIdle);
        if (!DialogPreviewHost.IsDialogOpen) throw new InvalidOperationException("Catalog dialog did not open.");
        ClosePreviewDialog_Click(DialogCancelButton, new RoutedEventArgs());
        if (DialogPreviewHost.IsDialogOpen) throw new InvalidOperationException("Catalog dialog did not close.");
        report.Add("Catalog dialog opens, closes and resolves its initial focus target");
        return report.ToArray();
    }

    private static T? FindVisual<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is T result) return result;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = FindVisual<T>(VisualTreeHelper.GetChild(root, i));
            if (child is not null) return child;
        }
        return null;
    }
}
