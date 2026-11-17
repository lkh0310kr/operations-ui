using System.Windows;
using Operations.UI.Theme;

namespace Operations.UiCatalog;

public partial class App : Application
{
    internal SystemContrastManager Contrast { get; private set; } = null!;
    protected override void OnStartup(StartupEventArgs e)
    {
        Contrast = new SystemContrastManager(Resources);
        base.OnStartup(e);
    }
    protected override void OnExit(ExitEventArgs e)
    {
        Contrast.Dispose();
        base.OnExit(e);
    }
}
