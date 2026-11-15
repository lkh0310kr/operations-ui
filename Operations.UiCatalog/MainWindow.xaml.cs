using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Operations.UiCatalog;

public partial class MainWindow : Window
{
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
            var sections = new (string FileName, string ElementName)[]
            {
                ("01-foundations.png", "FoundationSection"),
                ("02-controls.png", "ControlsSection"),
                ("03-feedback.png", "FeedbackSection"),
                ("04-operations.png", "OperationsSection"),
                ("05-data.png", "DataSection"),
                ("06-workspace.png", "WorkspaceSection"),
                ("07-dialogs.png", "DialogSection")
            };

            foreach (var (fileName, elementName) in sections)
            {
                if (FindName(elementName) is not FrameworkElement section) continue;
                CatalogScrollViewer.ScrollToVerticalOffset(section.TranslatePoint(new Point(0, 0), CatalogScrollViewer).Y + CatalogScrollViewer.VerticalOffset - 12);
                await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                await Task.Delay(80);
                await SaveSnapshotAsync(Path.Combine(outputDirectory, fileName));
            }

            Application.Current.Shutdown();
        }
    }

    private async Task SaveSnapshotAsync(string outputPath)
    {
        await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
        var dpi = VisualTreeHelper.GetDpi(this);
        var bitmap = new RenderTargetBitmap(Math.Max(1, (int)Math.Ceiling(ActualWidth * dpi.DpiScaleX)), Math.Max(1, (int)Math.Ceiling(ActualHeight * dpi.DpiScaleY)), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
        bitmap.Render(this);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        await using var output = File.Create(outputPath);
        encoder.Save(output);
    }

    private void NavigateToSection_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is string name && FindName(name) is FrameworkElement section)
            CatalogScrollViewer.ScrollToVerticalOffset(section.TranslatePoint(new Point(0, 0), CatalogScrollViewer).Y + CatalogScrollViewer.VerticalOffset - 24);
    }

    private void Notification_DismissRequested(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement notification) notification.Visibility = Visibility.Collapsed;
    }

    private void ClosePreviewDialog_Click(object sender, RoutedEventArgs e) => DialogPreviewHost.IsDialogOpen = false;

    private sealed record CatalogRow(string Name, string Area, string Status, string Updated);
}
