using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PomodoroTimer.Controls;

public sealed class TomatoIcon : Control
{
    public static readonly StyledProperty<IBrush?> TomatoBrushProperty =
        AvaloniaProperty.Register<TomatoIcon, IBrush?>(nameof(TomatoBrush));

    public static readonly StyledProperty<IBrush?> LeafBrushProperty =
        AvaloniaProperty.Register<TomatoIcon, IBrush?>(nameof(LeafBrush));

    public static readonly StyledProperty<IBrush?> HighlightBrushProperty =
        AvaloniaProperty.Register<TomatoIcon, IBrush?>(nameof(HighlightBrush));

    public IBrush? TomatoBrush
    {
        get => GetValue(TomatoBrushProperty);
        set => SetValue(TomatoBrushProperty, value);
    }

    public IBrush? LeafBrush
    {
        get => GetValue(LeafBrushProperty);
        set => SetValue(LeafBrushProperty, value);
    }

    public IBrush? HighlightBrush
    {
        get => GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 0)
        {
            return;
        }

        var center = new Point(Bounds.Width / 2, Bounds.Height * 0.58);
        var diameter = size * 0.68;
        var tomato = new Rect(center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter);
        context.DrawEllipse(TomatoBrush ?? Brushes.Transparent, null, tomato);

        var leaf = CreateLeafGeometry(size, center);
        context.DrawGeometry(LeafBrush ?? Brushes.Transparent, null, leaf);

        var stemPen = new Pen(LeafBrush ?? Brushes.Transparent, Math.Max(1.5, size * 0.045));
        context.DrawLine(
            stemPen,
            new Point(center.X + size * 0.02, center.Y - size * 0.38),
            new Point(center.X + size * 0.14, center.Y - size * 0.54));

        var highlightPen = new Pen(HighlightBrush ?? Brushes.Transparent, Math.Max(1.5, size * 0.045));
        context.DrawLine(
            highlightPen,
            new Point(center.X + size * 0.08, center.Y - size * 0.12),
            new Point(center.X + size * 0.25, center.Y - size * 0.29));
    }

    private static StreamGeometry CreateLeafGeometry(double size, Point center)
    {
        var geometry = new StreamGeometry();
        using var context = geometry.Open();

        context.BeginFigure(new Point(center.X - size * 0.20, center.Y - size * 0.38), isFilled: true);
        context.LineTo(new Point(center.X - size * 0.04, center.Y - size * 0.48));
        context.LineTo(new Point(center.X + size * 0.06, center.Y - size * 0.36));
        context.LineTo(new Point(center.X + size * 0.21, center.Y - size * 0.43));
        context.LineTo(new Point(center.X + size * 0.15, center.Y - size * 0.25));
        context.LineTo(new Point(center.X + size * 0.31, center.Y - size * 0.24));
        context.LineTo(new Point(center.X + size * 0.09, center.Y - size * 0.12));
        context.LineTo(new Point(center.X - size * 0.10, center.Y - size * 0.20));
        context.EndFigure(isClosed: true);

        return geometry;
    }
}
