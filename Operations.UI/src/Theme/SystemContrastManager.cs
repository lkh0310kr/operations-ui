using System.ComponentModel;
using System.Windows;

namespace Operations.UI.Theme;

/// <summary>Applies system-color mappings after the application theme and density resources.</summary>
public sealed class SystemContrastManager : IDisposable
{
    private readonly ResourceDictionary _resources;
    private ResourceDictionary? _overlay;
    private bool? _preview;

    public SystemContrastManager(ResourceDictionary resources)
    {
        _resources = resources;
        SystemParameters.StaticPropertyChanged += OnSystemPropertyChanged;
        Refresh();
    }

    /// <summary>Use null for Windows settings. A non-null value is intended for previews and tests.</summary>
    public void SetPreview(bool? enabled) { _preview = enabled; Refresh(); }

    private void OnSystemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(SystemParameters.HighContrast)) Refresh();
    }

    private void Refresh()
    {
        var enabled = _preview ?? SystemParameters.HighContrast;
        if (enabled && _overlay is null)
        {
            _overlay = new ResourceDictionary { Source = new Uri("pack://application:,,,/Operations.UI;component/src/Themes/Foundation/HighContrast.xaml", UriKind.Absolute) };
            _resources.MergedDictionaries.Add(_overlay);
        }
        else if (!enabled && _overlay is not null)
        {
            _resources.MergedDictionaries.Remove(_overlay);
            _overlay = null;
        }
    }

    public void Dispose()
    {
        SystemParameters.StaticPropertyChanged -= OnSystemPropertyChanged;
        if (_overlay is not null) _resources.MergedDictionaries.Remove(_overlay);
        _overlay = null;
    }
}
