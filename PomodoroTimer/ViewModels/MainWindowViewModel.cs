using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private readonly NavigationItemViewModel _timerNavigationItem;
    private readonly NavigationItemViewModel _statsNavigationItem;
    private readonly NavigationItemViewModel _settingsNavigationItem;
    private AppPage _selectedPage = AppPage.Timer;

    public MainWindowViewModel()
        : this(
            new AppSettings(),
            new InMemorySettingsStore(),
            new InMemorySessionStore(),
            new AppLocalizer(AppSettings.DefaultLanguageCode))
    {
    }

    public MainWindowViewModel(
        AppSettings settings,
        ISettingsStore settingsStore,
        ISessionStore sessionStore,
        AppLocalizer localizer,
        IClock? clock = null)
    {
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;

        NavigateCommand = new RelayCommand<AppPage>(Navigate);
        _timerNavigationItem = new NavigationItemViewModel(AppPage.Timer, _localizer.GetText(LocalizedText.NavTimer), NavigateCommand);
        _statsNavigationItem = new NavigationItemViewModel(AppPage.Stats, _localizer.GetText(LocalizedText.NavStats), NavigateCommand);
        _settingsNavigationItem = new NavigationItemViewModel(AppPage.Settings, _localizer.GetText(LocalizedText.NavSettings), NavigateCommand);

        NavigationItems =
        [
            _timerNavigationItem,
            _statsNavigationItem,
            _settingsNavigationItem,
        ];

        var timerService = new TimerService(settings, clock);
        Timer = new TimerViewModel(timerService, sessionStore, localizer);
        Stats = new StatsViewModel(sessionStore, localizer);
        Settings = new SettingsViewModel(settings, settingsStore, localizer);

        Settings.SettingsChanged += OnSettingsChanged;
        Timer.SessionsChanged += OnTimerSessionsChanged;

        UpdateNavigationSelection();
        _ = Timer.RefreshTodayAsync();
        _ = Stats.RefreshAsync();
    }

    public IRelayCommand<AppPage> NavigateCommand { get; }

    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; }

    public TimerViewModel Timer { get; }

    public StatsViewModel Stats { get; }

    public SettingsViewModel Settings { get; }

    public AppPage SelectedPage
    {
        get => _selectedPage;
        private set
        {
            if (SetProperty(ref _selectedPage, value))
            {
                UpdateNavigationSelection();
                OnPropertyChanged(nameof(CurrentPageViewModel));
            }
        }
    }

    public ViewModelBase CurrentPageViewModel => SelectedPage switch
    {
        AppPage.Stats => Stats,
        AppPage.Settings => Settings,
        _ => Timer,
    };

    public string WindowTitle => _localizer.GetText(LocalizedText.AppTitle);

    public string AppTitle => _localizer.GetText(LocalizedText.AppTitle);

    public string AppTagline => _localizer.GetText(LocalizedText.AppTagline);

    private void Navigate(AppPage page)
    {
        SelectedPage = page;
    }

    private void OnSettingsChanged(object? sender, AppSettings settings)
    {
        Timer.UpdateSettings(settings);
    }

    private async void OnTimerSessionsChanged(object? sender, EventArgs e)
    {
        await Stats.RefreshAsync().ConfigureAwait(true);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        _timerNavigationItem.Label = _localizer.GetText(LocalizedText.NavTimer);
        _statsNavigationItem.Label = _localizer.GetText(LocalizedText.NavStats);
        _settingsNavigationItem.Label = _localizer.GetText(LocalizedText.NavSettings);
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(AppTagline));
    }

    private void UpdateNavigationSelection()
    {
        _timerNavigationItem.IsSelected = SelectedPage == AppPage.Timer;
        _statsNavigationItem.IsSelected = SelectedPage == AppPage.Stats;
        _settingsNavigationItem.IsSelected = SelectedPage == AppPage.Settings;
    }
}
