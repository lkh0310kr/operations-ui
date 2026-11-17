using System.IO;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Operations.UiCatalog;

public partial class MainWindow : Window
{
    private int _buttonActivationCount;
    private bool _isLightTheme;
    private CancellationTokenSource? _demoCancellation;

    public MainWindow()
    {
        InitializeComponent();
        SampleGrid.ItemsSource = new[]
        {
            new CatalogRow("정기 점검", "조립 구역", "진행 중", "10:42"),
            new CatalogRow("부품 교체", "검수 구역", "완료", "10:31"),
            new CatalogRow("연결 확인", "포장 구역", "확인 필요", "10:18")
        };
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var arguments = Environment.GetCommandLineArgs();
        ApplyTheme(arguments.Any(argument => argument.Equals("--theme=light", StringComparison.OrdinalIgnoreCase)));
        var sizeArgument = arguments.FirstOrDefault(argument => argument.StartsWith("--size=", StringComparison.OrdinalIgnoreCase));
        if (sizeArgument is not null)
        {
            var dimensions = sizeArgument["--size=".Length..].Split('x', 'X');
            if (dimensions.Length == 2 && double.TryParse(dimensions[0], out var width) && double.TryParse(dimensions[1], out var height))
            {
                Width = width;
                Height = height;
                await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            }
        }
        var catalogVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-catalog=", StringComparison.OrdinalIgnoreCase));
        if (catalogVerification is not null)
        {
            if (RootTabs.Items.Count != 2 || ((TabItem)RootTabs.Items[0]).Header.ToString() != "Foundation" || ((TabItem)RootTabs.Items[1]).Header.ToString() != "Component")
                throw new InvalidOperationException("Catalog must have two top-level categories.");
            SelectCatalogSection(3);
            await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            var error = ValidationExample.Template.FindName("ErrorMessage", ValidationExample) as TextBlock;
            if (error?.Visibility != Visibility.Visible || string.IsNullOrEmpty(error.Text))
                throw new InvalidOperationException("Validation message without help text is not visible.");
            SelectCatalogSection(2);
            await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            BeginCancelable_Click(CancelableStart, new RoutedEventArgs());
            if (!CancelableStart.IsBusy || !CancelDemoButton.IsEnabled) throw new InvalidOperationException("Cancelable operation did not start.");
            CancelDemo_Click(CancelDemoButton, new RoutedEventArgs());
            await Task.Delay(100);
            if (CancelableStart.IsBusy || CancelDemoButton.IsEnabled || CancelableStatus.Text != "샘플 작업이 취소되었습니다.")
                throw new InvalidOperationException("Cancelable operation did not stop.");
            var path = Path.GetFullPath(catalogVerification["--verify-catalog=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, ["Two top-level categories", "Validation without help text is visible", "Busy operation can be cancelled separately"]);
            Application.Current.Shutdown();
            return;
        }
        var contrastVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-contrast=", StringComparison.OrdinalIgnoreCase));
        if (contrastVerification is not null)
        {
            var manager = ((App)Application.Current).Contrast;
            var erp = new ErpWindow { Owner = this };
            var hmi = new HmiWindow { Owner = this };
            erp.Show(); hmi.Show();
            var report = new List<string>();
            manager.SetPreview(true);
            foreach (var light in new[] { false, true })
            {
                ApplyTheme(light);
                await Dispatcher.InvokeAsync(() => { erp.UpdateLayout(); hmi.UpdateLayout(); }, DispatcherPriority.Render);
                var grid = (DataGrid)erp.FindName("InventoryGrid");
                var canvas = (SolidColorBrush)hmi.Background;
                var panel = (SolidColorBrush)grid.Background;
                var selection = (SolidColorBrush)erp.FindResource("SelectionBrush");
                var selectedText = (SolidColorBrush)erp.FindResource("SelectionTextBrush");
                var focus = (SolidColorBrush)erp.FindResource("FocusBrush");
                if (canvas.Color != SystemColors.WindowColor || panel.Color != SystemColors.WindowColor ||
                    selection.Color != SystemColors.HighlightColor || selectedText.Color != SystemColors.HighlightTextColor || focus.Color != SystemColors.HighlightColor)
                    throw new InvalidOperationException("System-color resources were not applied to the representative windows.");
                if (grid.MinRowHeight != 40 || ((Operations.UI.Controls.OperationButton)hmi.FindName("RequestButton")).MinHeight < 48)
                    throw new InvalidOperationException("Contrast preview changed density.");
                report.Add($"{(light ? "Light" : "Dark")}: system window, selection, selection text and focus colors; ERP standard and HMI touch sizes");
            }
            manager.SetPreview(false);
            await Dispatcher.InvokeAsync(erp.UpdateLayout, DispatcherPriority.Render);
            if (((SolidColorBrush)erp.Background).Color != Color.FromRgb(0xF4, 0xF5, 0xF7))
                throw new InvalidOperationException("Removing contrast preview did not restore Light theme.");
            report.Add("Removing preview restores Light theme");
            manager.SetPreview(null);
            var path = Path.GetFullPath(contrastVerification["--verify-contrast=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, report);
            erp.Close(); hmi.Close();
            Application.Current.Shutdown();
            return;
        }
        var erpVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-erp=", StringComparison.OrdinalIgnoreCase));
        if (erpVerification is not null)
        {
            var erp = new ErpWindow { Owner = this };
            erp.Show();
            await Dispatcher.InvokeAsync(erp.UpdateLayout, DispatcherPriority.Render);
            var report = await erp.VerifySampleBehaviorAsync();
            var path = Path.GetFullPath(erpVerification["--verify-erp=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, report);
            erp.Close();
            Application.Current.Shutdown();
            return;
        }
        var erpSnapshot = arguments.FirstOrDefault(argument => argument.StartsWith("--erp-snapshot=", StringComparison.OrdinalIgnoreCase));
        if (erpSnapshot is not null)
        {
            var erp = new ErpWindow { Owner = this };
            erp.Show();
            await Dispatcher.InvokeAsync(erp.UpdateLayout, DispatcherPriority.Render);
            await Task.Delay(200);
            var dpi = VisualTreeHelper.GetDpi(erp);
            var bitmap = new RenderTargetBitmap((int)Math.Ceiling(erp.ActualWidth * dpi.DpiScaleX), (int)Math.Ceiling(erp.ActualHeight * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            bitmap.Render(erp);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            var path = Path.GetFullPath(erpSnapshot["--erp-snapshot=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using (var stream = File.Create(path)) encoder.Save(stream);
            Application.Current.Shutdown();
            return;
        }
        var hmiVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-hmi=", StringComparison.OrdinalIgnoreCase));
        if (hmiVerification is not null)
        {
            var hmi = new HmiWindow { Owner = this };
            hmi.Show();
            await Dispatcher.InvokeAsync(hmi.UpdateLayout, DispatcherPriority.Render);
            var path = Path.GetFullPath(hmiVerification["--verify-hmi=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, hmi.VerifySampleBehavior());
            hmi.Close();
            Application.Current.Shutdown();
            return;
        }
        var hmiSnapshot = arguments.FirstOrDefault(argument => argument.StartsWith("--hmi-snapshot=", StringComparison.OrdinalIgnoreCase));
        if (hmiSnapshot is not null)
        {
            var hmi = new HmiWindow { Owner = this };
            hmi.Show();
            await Dispatcher.InvokeAsync(hmi.UpdateLayout, DispatcherPriority.Render);
            await Task.Delay(200);
            var dpi = VisualTreeHelper.GetDpi(hmi);
            var bitmap = new RenderTargetBitmap((int)Math.Ceiling(hmi.ActualWidth * dpi.DpiScaleX), (int)Math.Ceiling(hmi.ActualHeight * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            bitmap.Render(hmi);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            var path = Path.GetFullPath(hmiSnapshot["--hmi-snapshot=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using (var stream = File.Create(path)) encoder.Save(stream);
            Application.Current.Shutdown();
            return;
        }
        var mesVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-mes=", StringComparison.OrdinalIgnoreCase));
        if (mesVerification is not null)
        {
            var mes = new MesWindow { Owner = this };
            mes.Show();
            await Dispatcher.InvokeAsync(mes.UpdateLayout, DispatcherPriority.Render);
            var report = mes.VerifySampleBehavior();
            var path = Path.GetFullPath(mesVerification["--verify-mes=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, report);
            mes.Close();
            Application.Current.Shutdown();
            return;
        }
        var foundationVerification = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-foundation=", StringComparison.OrdinalIgnoreCase));
        if (foundationVerification is not null)
        {
            var report = new List<string>();
            var mes = new MesWindow { Owner = this };
            mes.Show();
            foreach (var light in new[] { false, true })
            {
                ApplyTheme(light);
                foreach (var (index, expectedRow, expectedControl) in new[] { (0, 32d, 32d), (1, 40d, 36d), (2, 52d, 48d) })
                {
                    DensityChoice.SelectedIndex = index;
                    await Dispatcher.InvokeAsync(mes.UpdateLayout, DispatcherPriority.Render);
                    var grid = (DataGrid)mes.FindName("OrdersGrid");
                    var search = (TextBox)mes.FindName("SearchBox");
                    if (grid.MinRowHeight != expectedRow || search.MinHeight != expectedControl || search.FontSize != (index == 2 ? 16 : 14))
                        throw new InvalidOperationException($"Density resources not applied: row={grid.MinRowHeight}, control={search.MinHeight}");
                    report.Add($"{(light ? "Light" : "Dark")} {((ComboBoxItem)DensityChoice.SelectedItem).Content}: row={grid.MinRowHeight}, control={search.MinHeight}, font={search.FontSize}");
                }
            }
            var path = Path.GetFullPath(foundationVerification["--verify-foundation=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, report);
            mes.Close();
            Application.Current.Shutdown();
            return;
        }
        var mesSnapshot = arguments.FirstOrDefault(argument => argument.StartsWith("--mes-snapshot=", StringComparison.OrdinalIgnoreCase));
        if (mesSnapshot is not null)
        {
            var mes = new MesWindow { Owner = this };
            mes.Show();
            await Dispatcher.InvokeAsync(mes.UpdateLayout, DispatcherPriority.Render);
            await Task.Delay(200);
            var dpi = VisualTreeHelper.GetDpi(mes);
            var bitmap = new RenderTargetBitmap((int)Math.Ceiling(mes.ActualWidth * dpi.DpiScaleX), (int)Math.Ceiling(mes.ActualHeight * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            bitmap.Render(mes);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            var path = Path.GetFullPath(mesSnapshot["--mes-snapshot=".Length..]);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using (var stream = File.Create(path)) encoder.Save(stream);
            Application.Current.Shutdown();
            return;
        }
        var interactionArgument = arguments.FirstOrDefault(argument => argument.StartsWith("--verify-interactions=", StringComparison.OrdinalIgnoreCase));
        if (interactionArgument is not null)
        {
            await VerifyInteractionsAsync(Path.GetFullPath(interactionArgument["--verify-interactions=".Length..]));
            Application.Current.Shutdown();
            return;
        }
        var snapshotArgument = arguments.FirstOrDefault(argument => argument.StartsWith("--snapshot=", StringComparison.OrdinalIgnoreCase));
        var snapshotDirectoryArgument = arguments.FirstOrDefault(argument => argument.StartsWith("--snapshot-dir=", StringComparison.OrdinalIgnoreCase));

        if (snapshotArgument is not null)
        {
            var outputPath = Path.GetFullPath(snapshotArgument["--snapshot=".Length..]);
            await SaveSnapshotAsync(outputPath);
            Application.Current.Shutdown();
            return;
        }

        if (snapshotDirectoryArgument is not null)
        {
            var outputDirectory = Path.GetFullPath(snapshotDirectoryArgument["--snapshot-dir=".Length..]);
            var sections = new (string FileName, int TabIndex)[]
            {
                ("01-foundations.png", 0),
                ("02-typography.png", 1),
                ("03-buttons.png", 2),
                ("04-inputs.png", 3),
                ("05-alerts.png", 4),
                ("06-status-metrics.png", 5),
                ("07-data.png", 6),
                ("08-layout.png", 7),
                ("09-dialogs.png", 8)
            };

            foreach (var (fileName, tabIndex) in sections)
            {
                SelectCatalogSection(tabIndex);
                await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                if (tabIndex == 2)
                {
                    FocusPreviewButton.Focus();
                    Keyboard.Focus(FocusPreviewButton);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                }
                else if (tabIndex == 3)
                {
                    FocusPreviewCombo.Focus();
                    Keyboard.Focus(FocusPreviewCombo);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                }
                else if (tabIndex == 6)
                {
                    SampleGrid.Focus();
                    Keyboard.Focus(SampleGrid);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                }
                await Task.Delay(80);
                await SaveSnapshotAsync(Path.Combine(outputDirectory, fileName));
                if (tabIndex == 3)
                {
                    FocusPreviewCheckBox.Focus();
                    Keyboard.Focus(FocusPreviewCheckBox);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                    await SaveSnapshotAsync(Path.Combine(outputDirectory, "04-inputs-checkbox-focus.png"));

                    FocusPreviewRadio.Focus();
                    Keyboard.Focus(FocusPreviewRadio);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                    await SaveSnapshotAsync(Path.Combine(outputDirectory, "04-inputs-radio-focus.png"));

                    FocusPreviewSwitch.Focus();
                    Keyboard.Focus(FocusPreviewSwitch);
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                    await SaveSnapshotAsync(Path.Combine(outputDirectory, "04-inputs-switch-focus.png"));

                    InputScrollViewer.ScrollToEnd();
                    await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                    await SaveSnapshotAsync(Path.Combine(outputDirectory, "04-inputs-toggle-states.png"));
                }
            }

            Application.Current.Shutdown();
        }
    }

    private void DensityChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { SelectedItem: ComboBoxItem item } || item.Content is not string density) return;
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var dictionary = dictionaries.FirstOrDefault(d => d.Source?.OriginalString.Contains("/Foundation/Density.", StringComparison.OrdinalIgnoreCase) == true);
        if (dictionary is not null)
            dictionary.Source = new Uri($"pack://application:,,,/Operations.UI;component/src/Themes/Foundation/Density.{density}.xaml", UriKind.Absolute);
    }

    private void OpenMes_Click(object sender, RoutedEventArgs e) => new MesWindow { Owner = this }.Show();
    private void OpenHmi_Click(object sender, RoutedEventArgs e) => new HmiWindow { Owner = this }.Show();
    private void OpenErp_Click(object sender, RoutedEventArgs e) => new ErpWindow { Owner = this }.Show();

    private async void BeginCancelable_Click(object sender, RoutedEventArgs e)
    {
        if (_demoCancellation is not null) return;
        using var cancellation = new CancellationTokenSource();
        _demoCancellation = cancellation;
        CancelableStart.IsBusy = true;
        CancelDemoButton.IsEnabled = true;
        CancelableStatus.Text = "샘플 작업 진행 중 · 취소할 수 있습니다.";
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellation.Token);
            CancelableStatus.Text = "샘플 작업 완료";
        }
        catch (OperationCanceledException) { CancelableStatus.Text = "샘플 작업이 취소되었습니다."; }
        finally
        {
            _demoCancellation = null;
            CancelableStart.IsBusy = false;
            CancelDemoButton.IsEnabled = false;
        }
    }

    private void CancelDemo_Click(object sender, RoutedEventArgs e) => _demoCancellation?.Cancel();

    private void ThemeToggle_Click(object sender, RoutedEventArgs e) => ApplyTheme(!_isLightTheme);

    private void ApplyTheme(bool useLightTheme)
    {
        var designSystem = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(dictionary => dictionary.Source?.OriginalString.Contains("/src/Themes/DesignSystem", StringComparison.OrdinalIgnoreCase) == true);
        if (designSystem is null) return;

        var themeFile = useLightTheme ? "DesignSystem.Light.xaml" : "DesignSystem.xaml";
        designSystem.Source = new Uri($"pack://application:,,,/Operations.UI;component/src/Themes/{themeFile}", UriKind.Absolute);
        _isLightTheme = useLightTheme;
        ThemeToggle.Content = useLightTheme ? "다크 테마" : "라이트 테마";
        AutomationProperties.SetName(ThemeToggle, useLightTheme ? "다크 테마로 전환" : "라이트 테마로 전환");
        ThemeBadgeLabel.Text = useLightTheme ? "밝은 테마" : "어두운 테마";
        ThemeDescription.Text = useLightTheme
            ? "화면 계층과 정보 상태를 구분하는 라이트 테마 토큰"
            : "화면 계층과 정보 상태를 구분하는 다크 테마 토큰";
        CanvasHex.Text = FormatTokenColor("CanvasColor");
        SurfaceHex.Text = FormatTokenColor("SurfaceColor");
        RaisedHex.Text = FormatTokenColor("SurfaceRaisedColor");
        AccentHex.Text = FormatTokenColor("AccentColor");
        SuccessHex.Text = FormatTokenColor("SuccessColor");
        DangerHex.Text = FormatTokenColor("DangerColor");
    }

    private static string FormatTokenColor(string resourceKey) =>
        Application.Current.FindResource(resourceKey) is System.Windows.Media.Color color
            ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
            : string.Empty;

    private async Task SaveSnapshotAsync(string outputPath)
    {
        await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
        var dpi = VisualTreeHelper.GetDpi(this);
        var content = (FrameworkElement)Content;
        var contentSize = new Size(content.ActualWidth, content.ActualHeight);
        var bitmap = new RenderTargetBitmap(Math.Max(1, (int)Math.Ceiling(contentSize.Width * dpi.DpiScaleX)), Math.Max(1, (int)Math.Ceiling(contentSize.Height * dpi.DpiScaleY)), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
        bitmap.Render(this);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        await using var output = File.Create(outputPath);
        encoder.Save(output);
    }

    private void NavigateToSection_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is string indexText && int.TryParse(indexText, out var index))
            SelectCatalogSection(index);
    }

    private void SelectCatalogSection(int index)
    {
        RootTabs.SelectedIndex = index < 2 ? 0 : 1;
        if (index < 2) FoundationTabs.SelectedIndex = index;
        else ComponentTabs.SelectedIndex = index - 2;
    }

    private void Notification_DismissRequested(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement notification) notification.Visibility = Visibility.Collapsed;
    }

    private void DemoButton_Click(object sender, RoutedEventArgs e)
    {
        ButtonInteractionStatus.Text = $"Button activated: {++_buttonActivationCount}";
    }

    private void TextLink_Click(object sender, RoutedEventArgs e)
    {
        TextLinkStatus.Text = "도움말 링크가 활성화되었습니다.";
    }

    private void ClosePreviewDialog_Click(object sender, RoutedEventArgs e) => DialogPreviewHost.IsDialogOpen = false;

    private async void RetryData_Click(object sender, RoutedEventArgs e)
    {
        RetryDataButton.IsEnabled = false;
        RetryStatePanel.State = Operations.UI.Controls.DataStateKind.Loading;
        RetryStatePanel.Title = "목록을 다시 확인하는 중";
        RetryStatePanel.Message = "연결을 확인하고 있습니다.";
        await Task.Delay(800);
        RetryStatePanel.State = Operations.UI.Controls.DataStateKind.Error;
        RetryStatePanel.Title = "목록을 불러오지 못했습니다";
        RetryStatePanel.Message = "연결 상태를 확인한 뒤 다시 시도하세요.";
        RetryDataButton.IsEnabled = true;
    }

    private sealed record CatalogRow(string Name, string Area, string Status, string Updated);
}
