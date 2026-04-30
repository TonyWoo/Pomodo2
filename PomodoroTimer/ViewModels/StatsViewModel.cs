using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;

namespace PomodoroTimer.ViewModels;

public sealed class StatsViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private IReadOnlyList<FocusSession> _sessions;

    public StatsViewModel(AppLocalizer localizer, IEnumerable<FocusSession> sessions)
    {
        _localizer = localizer;
        _sessions = sessions.ToList();
        TodaySessions = [];
        RefreshSessions(_sessions);
    }

    public ObservableCollection<SessionListItemViewModel> TodaySessions { get; }

    public string Title => _localizer.GetText(LocalizedText.StatsTodayFocusStats);

    public string PomodorosLabel => _localizer.GetText(LocalizedText.StatsTodayPomodoros);

    public string FocusMinutesLabel => _localizer.GetText(LocalizedText.StatsFocusMinutes);

    public string CompletedSummary => _localizer.Format(LocalizedText.StatsCompletedFormat, TodayStats.CompletedPomodoros);

    public string FocusMinutesSummary => _localizer.Format(LocalizedText.StatsFocusMinutesFormat, TodayStats.TotalFocusMinutes);

    public string CompletedValue => TodayStats.CompletedPomodoros.ToString(CultureInfo.InvariantCulture);

    public string FocusMinutesValue => TodayStats.TotalFocusMinutes.ToString(CultureInfo.InvariantCulture);

    public string EmptySessionsText => _localizer.GetText(LocalizedText.StatsNoSessions);

    public string SessionTopicHeader => _localizer.GetText(LocalizedText.SessionTopic);

    public string SessionCompletedAtHeader => _localizer.GetText(LocalizedText.SessionCompletedAt);

    public string SessionDurationHeader => _localizer.GetText(LocalizedText.SessionDuration);

    public string SessionStatusHeader => _localizer.GetText(LocalizedText.SessionStatus);

    public bool HasTodaySessions => TodaySessions.Count > 0;

    public bool HasNoTodaySessions => !HasTodaySessions;

    private DailyStats TodayStats => DailyStats.FromSessions(DateOnly.FromDateTime(DateTime.Now), _sessions);

    public void RefreshSessions(IEnumerable<FocusSession> sessions)
    {
        _sessions = sessions.ToList();
        TodaySessions.Clear();

        var today = DateOnly.FromDateTime(DateTime.Now);
        foreach (var session in _sessions
                     .Where(session => DateOnly.FromDateTime(session.EndedAt.LocalDateTime) == today)
                     .OrderByDescending(session => session.EndedAt))
        {
            TodaySessions.Add(SessionListItemViewModel.FromSession(session, _localizer));
        }

        RaiseAllChanged();
    }

    public void RefreshLocalization()
    {
        RefreshSessions(_sessions);
    }

    private void RaiseAllChanged()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(PomodorosLabel));
        OnPropertyChanged(nameof(FocusMinutesLabel));
        OnPropertyChanged(nameof(CompletedSummary));
        OnPropertyChanged(nameof(FocusMinutesSummary));
        OnPropertyChanged(nameof(CompletedValue));
        OnPropertyChanged(nameof(FocusMinutesValue));
        OnPropertyChanged(nameof(EmptySessionsText));
        OnPropertyChanged(nameof(SessionTopicHeader));
        OnPropertyChanged(nameof(SessionCompletedAtHeader));
        OnPropertyChanged(nameof(SessionDurationHeader));
        OnPropertyChanged(nameof(SessionStatusHeader));
        OnPropertyChanged(nameof(HasTodaySessions));
        OnPropertyChanged(nameof(HasNoTodaySessions));
    }
}
