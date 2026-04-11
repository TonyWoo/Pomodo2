using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
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
            var localizer = new AppLocalizer(new FileLanguagePreferenceStore());

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new PomodoroTimerState(), localizer),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
