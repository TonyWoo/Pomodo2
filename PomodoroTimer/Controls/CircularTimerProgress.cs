using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PomodoroTimer.Controls;

public sealed class CircularTimerProgress : Control
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<CircularTimerProgress, double>(nameof(Progress));

    public static readonly StyledProperty<IBrush> TrackBrushProperty =
        AvaloniaProperty.Register<CircularTimerProgress, IBrush>(nameof(TrackBrush), Brushes.Gainsboro);

    public static readonly StyledProperty<IBrush> ProgressBrushProperty =
        AvaloniaProperty.Register<CircularTimerProgress, IBrush>(nameof(ProgressBrush), Brushes.Red);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<CircularTimerProgress, double>(nameof(StrokeThickness), 14);

    static CircularTimerProgress()
    {
        AffectsRender<CircularTimerProgress>(
            ProgressProperty,
            TrackBrushProperty,
            ProgressBrushProperty,
            StrokeThicknessProperty);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public IBrush TrackBrush
    {
        get => GetValue(TrackBrushProperty);
        set => SetValue(TrackBrushProperty, value);
    }

    public IBrush ProgressBrush
    {
        get => GetValue(ProgressBrushProperty);
        set => SetValue(ProgressBrushProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0)
        {
            return;
        }

        var thickness = Math.Max(1, StrokeThickness);
        var rect = new Rect(
            (Bounds.Width - side) / 2 + thickness / 2,
            (Bounds.Height - side) / 2 + thickness / 2,
            side - thickness,
            side - thickness);

        context.DrawEllipse(null, new Pen(TrackBrush, thickness), rect);

        var progress = Math.Clamp(Progress, 0, 1);
        if (progress <= 0)
        {
            return;
        }

        if (progress >= 0.999)
        {
            context.DrawEllipse(null, new Pen(ProgressBrush, thickness), rect);
            return;
        }

        var center = rect.Center;
        var radius = rect.Width / 2;
        var startAngle = -90d;
        var endAngle = startAngle + progress * 360d;
        var start = PointOnCircle(center, radius, startAngle);
        var end = PointOnCircle(center, radius, endAngle);

        var geometry = new StreamGeometry();
        using (var geometryContext = geometry.Open())
        {
            geometryContext.BeginFigure(start, isFilled: false);
            geometryContext.ArcTo(
                end,
                new Size(radius, radius),
                rotationAngle: 0,
                isLargeArc: progress > 0.5,
                sweepDirection: SweepDirection.Clockwise);
        }

        context.DrawGeometry(null, new Pen(ProgressBrush, thickness), geometry);
    }

    private static Point PointOnCircle(Point center, double radius, double angleDegrees)
    {
        var angle = angleDegrees * Math.PI / 180d;
        return new Point(
            center.X + radius * Math.Cos(angle),
            center.Y + radius * Math.Sin(angle));
    }
}
