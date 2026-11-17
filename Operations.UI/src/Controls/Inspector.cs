using System.Windows;
using System.Windows.Controls;

namespace Operations.UI.Controls;

/// <summary>A scrollable supporting detail area. Editing and selection remain the consumer's responsibility.</summary>
public sealed class Inspector : HeaderedContentControl
{
    static Inspector() => DefaultStyleKeyProperty.OverrideMetadata(
        typeof(Inspector), new FrameworkPropertyMetadata(typeof(Inspector)));
}
