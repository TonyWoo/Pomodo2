using Avalonia.Controls;

namespace PomodoroTimer.Views;

public partial class MainView : UserControl
{
    private const double CompactBreakpoint = 700;

    public MainView()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var useCompactNavigation = Bounds.Width < CompactBreakpoint;
        DesktopNav.IsVisible = !useCompactNavigation;
        CompactNav.IsVisible = useCompactNavigation;
    }
}
