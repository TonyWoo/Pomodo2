using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PomodoroTimer.Localization;
using PomodoroTimer.Services;
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
        var pathProvider = new AppDataPathProvider();
        var settingsStore = new JsonSettingsStore(pathProvider);
        var sessionStore = new JsonSessionStore(pathProvider);
        var settings = settingsStore.LoadAsync().GetAwaiter().GetResult();
        var localizer = new AppLocalizer(settings.LanguageCode);

        return new MainWindowViewModel(settings, settingsStore, sessionStore, localizer);
    }

    private static MainView CreateMainView()
    {
        return new MainView
        {
            DataContext = CreateMainWindowViewModel(),
        };
    }
}
