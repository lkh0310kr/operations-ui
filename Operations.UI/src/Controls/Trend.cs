using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace Operations.UI.Controls;

/// <summary>Source-provided quality; the chart never determines freshness or repairs a missing sample.</summary>
public enum SampleQuality { Good, Uncertain, Bad }
public sealed record TrendSample(DateTimeOffset Time, double? Value, SampleQuality Quality);

/// <summary>Renders supplied samples in the supplied time/value domain. Null and bad samples break the line.</summary>
public sealed class Trend : FrameworkElement
{
    public Trend()
    {
        SetResourceReference(LineBrushProperty, "Ops.Brush.Interaction.Focus");
        SetResourceReference(GridBrushProperty, "Ops.Brush.Border.Subtle");
        ClipToBounds = true;
    }

    public static readonly DependencyProperty SamplesProperty = DependencyProperty.Register(nameof(Samples), typeof(IReadOnlyList<TrendSample>), typeof(Trend), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(DateTimeOffset), typeof(Trend), new FrameworkPropertyMetadata(default(DateTimeOffset), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty EndProperty = DependencyProperty.Register(nameof(End), typeof(DateTimeOffset), typeof(Trend), new FrameworkPropertyMetadata(default(DateTimeOffset), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(Trend), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(Trend), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(Trend), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty GridBrushProperty = DependencyProperty.Register(nameof(GridBrush), typeof(Brush), typeof(Trend), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
    public IReadOnlyList<TrendSample>? Samples { get => (IReadOnlyList<TrendSample>?)GetValue(SamplesProperty); set => SetValue(SamplesProperty, value); }
    public DateTimeOffset Start { get => (DateTimeOffset)GetValue(StartProperty); set => SetValue(StartProperty, value); }
    public DateTimeOffset End { get => (DateTimeOffset)GetValue(EndProperty); set => SetValue(EndProperty, value); }
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public Brush LineBrush { get => (Brush)GetValue(LineBrushProperty); set => SetValue(LineBrushProperty, value); }
    public Brush GridBrush { get => (Brush)GetValue(GridBrushProperty); set => SetValue(GridBrushProperty, value); }

    protected override AutomationPeer OnCreateAutomationPeer() => new TrendAutomationPeer(this);

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        if (ActualWidth <= 0 || ActualHeight <= 0 || End <= Start || !double.IsFinite(Minimum) || !double.IsFinite(Maximum) || Maximum <= Minimum) return;
        var gridPen = new Pen(GridBrush, 1);
        for (int i = 0; i <= 4; i++)
        {
            var y = ActualHeight * i / 4;
            dc.DrawLine(gridPen, new Point(0, y), new Point(ActualWidth, y));
        }
        if (Samples is null) return;
        Point? previous = null;
        var linePen = new Pen(LineBrush, 2);
        var duration = (End - Start).TotalSeconds;
        foreach (var sample in Samples)
        {
            if (sample.Quality != SampleQuality.Good || sample.Value is not double value || !double.IsFinite(value)) { previous = null; continue; }
            var point = new Point((sample.Time - Start).TotalSeconds / duration * ActualWidth,
                                  ActualHeight - (value - Minimum) / (Maximum - Minimum) * ActualHeight);
            if (previous is Point from) dc.DrawLine(linePen, from, point);
            previous = point;
        }
    }
}

internal sealed class TrendAutomationPeer(Trend owner) : FrameworkElementAutomationPeer(owner)
{
    protected override string GetClassNameCore() => nameof(Trend);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Image;
}
