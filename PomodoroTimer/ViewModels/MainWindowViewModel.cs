using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private AppPage _currentPage = AppPage.Timer;
    private bool _isCompactLayout;

    public MainWindowViewModel()
        : this(
            new AppSettings(),
            new AppLocalizer("zh-Hans"),
            new JsonSettingsStore(new AppDataPathProvider()),
            new JsonSessionStore(new AppDataPathProvider()),
            new JsonTaskStore(new AppDataPathProvider()),
            [],
            [])
    {
    }

    public MainWindowViewModel(
        AppSettings settings,
        AppLocalizer localizer,
        ISettingsStore settingsStore,
        ISessionStore sessionStore,
        ITaskStore taskStore,
        IEnumerable<FocusSession> sessions,
        IEnumerable<TodayTask> tasks)
    {
        _localizer = localizer;
        var timerService = new TimerService(settings);
        Timer = new TimerViewModel(timerService, localizer, sessionStore, taskStore, sessions, tasks);
        Stats = new StatsViewModel(localizer, Timer.AllSessions);
        Settings = new SettingsViewModel(settings, localizer, settingsStore);
        CurrentPageViewModel = Timer;

        NavigateCommand = new RelayCommand<string>(Navigate);

        Timer.SessionsChanged += (_, updatedSessions) => Stats.RefreshSessions(updatedSessions);
        Settings.SettingsChanged += (_, _) => Timer.ApplySettings();
        _localizer.LanguageChanged += (_, _) => RefreshLocalization();
    }

    public IRelayCommand<string> NavigateCommand { get; }

    public TimerViewModel Timer { get; }

    public StatsViewModel Stats { get; }

    public SettingsViewModel Settings { get; }

    public ViewModelBase CurrentPageViewModel { get; private set; }

    public string WindowTitle => _localizer.GetText(LocalizedText.AppTitle);

    public string NavTimerText => _localizer.GetText(LocalizedText.NavTimer);

    public string NavStatsText => _localizer.GetText(LocalizedText.NavStats);

    public string NavSettingsText => _localizer.GetText(LocalizedText.NavSettings);

    public bool IsTimerPage => _currentPage == AppPage.Timer;

    public bool IsStatsPage => _currentPage == AppPage.Stats;

    public bool IsSettingsPage => _currentPage == AppPage.Settings;

    public bool IsCompactLayout
    {
        get => _isCompactLayout;
        set
        {
            if (SetProperty(ref _isCompactLayout, value))
            {
                Timer.IsCompactLayout = value;
                OnPropertyChanged(nameof(IsWideLayout));
            }
        }
    }

    public bool IsWideLayout => !IsCompactLayout;

    private void Navigate(string? pageName)
    {
        if (!Enum.TryParse<AppPage>(pageName, ignoreCase: true, out var nextPage))
        {
            return;
        }

        if (_currentPage == nextPage)
        {
            return;
        }

        _currentPage = nextPage;
        CurrentPageViewModel = nextPage switch
        {
            AppPage.Stats => Stats,
            AppPage.Settings => Settings,
            _ => Timer,
        };

        OnPropertyChanged(nameof(CurrentPageViewModel));
        OnPropertyChanged(nameof(IsTimerPage));
        OnPropertyChanged(nameof(IsStatsPage));
        OnPropertyChanged(nameof(IsSettingsPage));
    }

    private void RefreshLocalization()
    {
        Timer.RefreshLocalization();
        Stats.RefreshLocalization();
        Settings.RefreshLocalization();

        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(NavTimerText));
        OnPropertyChanged(nameof(NavStatsText));
        OnPropertyChanged(nameof(NavSettingsText));
    }
}
