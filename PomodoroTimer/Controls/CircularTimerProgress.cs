using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PomodoroTimer.Controls;

public sealed class CircularTimerProgress : Control
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<CircularTimerProgress, double>(nameof(Progress));

    public static readonly StyledProperty<IBrush?> TrackBrushProperty =
        AvaloniaProperty.Register<CircularTimerProgress, IBrush?>(nameof(TrackBrush));

    public static readonly StyledProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.Register<CircularTimerProgress, IBrush?>(nameof(IndicatorBrush));

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<CircularTimerProgress, double>(nameof(StrokeThickness), 16);

    static CircularTimerProgress()
    {
        AffectsRender<CircularTimerProgress>(
            ProgressProperty,
            TrackBrushProperty,
            IndicatorBrushProperty,
            StrokeThicknessProperty);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public IBrush? TrackBrush
    {
        get => GetValue(TrackBrushProperty);
        set => SetValue(TrackBrushProperty, value);
    }

    public IBrush? IndicatorBrush
    {
        get => GetValue(IndicatorBrushProperty);
        set => SetValue(IndicatorBrushProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 0)
        {
            return;
        }

        var thickness = Math.Max(1, StrokeThickness);
        var radius = Math.Max(0, (size - thickness) / 2);
        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var trackPen = new Pen(TrackBrush ?? Brushes.Transparent, thickness, lineCap: PenLineCap.Round);
        context.DrawEllipse(null, trackPen, center, radius, radius);

        var progress = Math.Clamp(Progress, 0, 1);
        if (progress <= 0)
        {
            return;
        }

        var indicatorPen = new Pen(IndicatorBrush ?? Brushes.Black, thickness, lineCap: PenLineCap.Round);
        if (progress >= 0.999)
        {
            context.DrawEllipse(null, indicatorPen, center, radius, radius);
            return;
        }

        var startAngle = -Math.PI / 2;
        var endAngle = startAngle + (Math.PI * 2 * progress);
        var start = PointOnCircle(center, radius, startAngle);
        var end = PointOnCircle(center, radius, endAngle);
        var isLargeArc = progress > 0.5;

        var geometry = new StreamGeometry();
        using (var geometryContext = geometry.Open())
        {
            geometryContext.BeginFigure(start, isFilled: false);
            geometryContext.ArcTo(
                end,
                new Size(radius, radius),
                rotationAngle: 0,
                isLargeArc,
                SweepDirection.Clockwise);
        }

        context.DrawGeometry(null, indicatorPen, geometry);
    }

    private static Point PointOnCircle(Point center, double radius, double angle)
    {
        return new Point(
            center.X + radius * Math.Cos(angle),
            center.Y + radius * Math.Sin(angle));
    }
}
