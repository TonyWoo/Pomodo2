using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using PomodoroTimer.ViewModels;

namespace PomodoroTimer.Views;

public partial class TimerView : UserControl
{
    public TimerView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is TimerViewModel vm)
        {
            vm.ConfirmDeleteTask = ShowConfirmDialogAsync;
        }
    }

    private async Task<bool> ShowConfirmDialogAsync(string title, string message)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return false;

        var tcs = new TaskCompletionSource<bool>();

        var confirmBtn = new Button { Content = "✓", MinWidth = 60 };
        var cancelBtn  = new Button { Content = "✗", MinWidth = 60 };

        var dialog = new Window
        {
            Title = title,
            Width = 320,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(24, 20),
                Spacing = 20,
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 14
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { cancelBtn, confirmBtn }
                    }
                }
            }
        };

        confirmBtn.Click += (_, _) => { tcs.TrySetResult(true);  dialog.Close(); };
        cancelBtn.Click  += (_, _) => { tcs.TrySetResult(false); dialog.Close(); };
        dialog.Closed    += (_, _) => tcs.TrySetResult(false);

        if (topLevel is Window ownerWindow)
            await dialog.ShowDialog(ownerWindow);
        else
            dialog.Show();

        return await tcs.Task;
    }
}
