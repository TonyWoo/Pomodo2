using Avalonia.Controls;
using PomodoroTimer.ViewModels;

namespace PomodoroTimer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.IsCompactLayout = Bounds.Width < 700;
        }
    }
}
