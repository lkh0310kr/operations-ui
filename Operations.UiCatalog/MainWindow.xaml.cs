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
            new CatalogRow("capture_001.jpg", "미지정", "—"),
            new CatalogRow("capture_002.jpg", "정상", "0.284"),
            new CatalogRow("capture_003.jpg", "불량", "1.462")
        };
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var snapshotArgument = Environment.GetCommandLineArgs().FirstOrDefault(argument => argument.StartsWith("--snapshot=", StringComparison.OrdinalIgnoreCase));
        if (snapshotArgument is null) return;
        var outputPath = Path.GetFullPath(snapshotArgument["--snapshot=".Length..]);
        await Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
        var dpi = VisualTreeHelper.GetDpi(this);
        var bitmap = new RenderTargetBitmap(Math.Max(1, (int)Math.Ceiling(ActualWidth * dpi.DpiScaleX)), Math.Max(1, (int)Math.Ceiling(ActualHeight * dpi.DpiScaleY)), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
        bitmap.Render(this);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        await using var output = File.Create(outputPath);
        encoder.Save(output);
        Application.Current.Shutdown();
    }

    private void NavigateToSection_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is string name && FindName(name) is FrameworkElement section)
            CatalogScrollViewer.ScrollToVerticalOffset(section.TranslatePoint(new Point(0, 0), CatalogScrollViewer).Y + CatalogScrollViewer.VerticalOffset - 24);
    }

    private sealed record CatalogRow(string FileName, string Label, string Score);
}
