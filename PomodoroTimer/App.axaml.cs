using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.ViewModels;
using PomodoroTimer.Views;

namespace PomodoroTimer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = CreateMainWindowViewModel(),
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
        {
            activityLifetime.MainViewFactory = CreateMainView;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
        {
            singleViewLifetime.MainView = CreateMainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static MainWindowViewModel CreateMainWindowViewModel()
    {
        var localizer = new AppLocalizer(new FileLanguagePreferenceStore());
        return new MainWindowViewModel(new PomodoroTimerState(), localizer);
    }

    private static MainView CreateMainView()
    {
        return new MainView
        {
            DataContext = CreateMainWindowViewModel(),
        };
    }
}
