using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class StatsViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private readonly ISessionStore _sessionStore;
    private DateTime _currentDate = DateTime.Now.Date;
    private int _completedPomodoros;
    private int _focusMinutes;

    public StatsViewModel(ISessionStore sessionStore, AppLocalizer localizer)
    {
        _sessionStore = sessionStore;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;
    }

    public ObservableCollection<SessionRowViewModel> Sessions { get; } = [];

    public int CompletedPomodoros
    {
        get => _completedPomodoros;
        private set => SetProperty(ref _completedPomodoros, value);
    }

    public int FocusMinutes
    {
        get => _focusMinutes;
        private set => SetProperty(ref _focusMinutes, value);
    }

    public string PageTitle => _localizer.GetText(LocalizedText.StatsPageTitle);

    public string TodayPomodorosLabel => _localizer.GetText(LocalizedText.StatsTodayPomodoros);

    public string CompletedLabel => _localizer.GetText(LocalizedText.StatsCompleted);

    public string FocusMinutesLabel => _localizer.GetText(LocalizedText.StatsFocusMinutes);

    public string TodayFocusStatsLabel => _localizer.GetText(LocalizedText.StatsTodayFocusStats);

    public string SummaryText => _localizer.Format(LocalizedText.StatsSummaryFormat, CompletedPomodoros);

    public string SessionsTitle => _localizer.GetText(LocalizedText.StatsSessionsTitle);

    public string NoSessionsText => _localizer.GetText(LocalizedText.TimerNoSessions);

    public string TopicHeader => _localizer.GetText(LocalizedText.TimerTaskTopicHeader);

    public string CompletedTimeHeader => _localizer.GetText(LocalizedText.TimerTaskCompletedTimeHeader);

    public string DurationHeader => _localizer.GetText(LocalizedText.TimerTaskDurationHeader);

    public string StatusHeader => _localizer.GetText(LocalizedText.TimerTaskStatusHeader);

    public bool HasSessions => Sessions.Count > 0;

    public bool ShowEmptyState => !HasSessions;

    public async Task RefreshAsync()
    {
        _currentDate = DateTime.Now.Date;
        var sessions = await _sessionStore.GetSessionsByDateAsync(_currentDate).ConfigureAwait(true);
        var completed = sessions.Where(session => session.Completed).ToList();

        Sessions.Clear();
        foreach (var session in sessions)
        {
            Sessions.Add(CreateRow(session));
        }

        CompletedPomodoros = completed.Count;
        FocusMinutes = completed.Sum(session => session.PlannedMinutes);
        RaiseDerivedProperties();
    }

    private SessionRowViewModel CreateRow(FocusSession session)
    {
        var localEnd = session.EndedAt.ToLocalTime();
        var status = session.Completed
            ? _localizer.GetText(LocalizedText.StatusCompleted)
            : _localizer.GetText(LocalizedText.StatusIncomplete);

        return new SessionRowViewModel(
            session.Topic,
            localEnd.ToString("HH:mm", CultureInfo.InvariantCulture),
            $"{session.PlannedMinutes} {_localizer.GetText(LocalizedText.SettingsMinutesSuffix)}",
            status,
            session.Completed);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RaiseLocalizedProperties();
        _ = RefreshAsync();
    }

    private void RaiseLocalizedProperties()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(TodayPomodorosLabel));
        OnPropertyChanged(nameof(CompletedLabel));
        OnPropertyChanged(nameof(FocusMinutesLabel));
        OnPropertyChanged(nameof(TodayFocusStatsLabel));
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(SessionsTitle));
        OnPropertyChanged(nameof(NoSessionsText));
        OnPropertyChanged(nameof(TopicHeader));
        OnPropertyChanged(nameof(CompletedTimeHeader));
        OnPropertyChanged(nameof(DurationHeader));
        OnPropertyChanged(nameof(StatusHeader));
    }

    private void RaiseDerivedProperties()
    {
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(HasSessions));
        OnPropertyChanged(nameof(ShowEmptyState));
    }
}
