using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class TimerViewModel : ViewModelBase
{
    private static readonly TimeSpan UiTickInterval = TimeSpan.FromMilliseconds(500);
    private readonly AppLocalizer _localizer;
    private readonly ISessionStore _sessionStore;
    private readonly DispatcherTimer _uiTimer;
    private readonly TimerService _timerService;
    private DateTime _currentDate = DateTime.Now.Date;
    private string _currentTopic = string.Empty;
    private int _todayPomodoros;
    private int _todayFocusMinutes;

    public TimerViewModel(TimerService timerService, ISessionStore sessionStore, AppLocalizer localizer)
    {
        _timerService = timerService;
        _sessionStore = sessionStore;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;

        StartCommand = new RelayCommand(Start);
        PauseCommand = new RelayCommand(Pause);
        ResumeCommand = new RelayCommand(Resume);
        ResetCommand = new RelayCommand(Reset);

        _uiTimer = new DispatcherTimer { Interval = UiTickInterval };
        _uiTimer.Tick += OnTimerTick;
        _uiTimer.Start();
    }

    public event EventHandler? SessionsChanged;

    public IRelayCommand StartCommand { get; }

    public IRelayCommand PauseCommand { get; }

    public IRelayCommand ResumeCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public ObservableCollection<SessionRowViewModel> Sessions { get; } = [];

    public string CurrentTopic
    {
        get => _currentTopic;
        set => SetProperty(ref _currentTopic, value);
    }

    public int TodayPomodoros
    {
        get => _todayPomodoros;
        private set => SetProperty(ref _todayPomodoros, value);
    }

    public int TodayFocusMinutes
    {
        get => _todayFocusMinutes;
        private set => SetProperty(ref _todayFocusMinutes, value);
    }

    public string PageTitle => _localizer.GetText(LocalizedText.TimerPageTitle);

    public string ModeText => _timerService.Mode == TimerMode.Work
        ? _localizer.GetText(LocalizedText.TimerWork)
        : _localizer.GetText(LocalizedText.TimerBreak);

    public string StatusText => _timerService.Status switch
    {
        TimerStatus.Running when _timerService.Mode == TimerMode.Break => _localizer.GetText(LocalizedText.TimerBreakRunning),
        TimerStatus.Running => _localizer.GetText(LocalizedText.TimerRunning),
        TimerStatus.Paused => _localizer.GetText(LocalizedText.TimerPaused),
        TimerStatus.Completed when _timerService.LastCompletionKind == TimerCompletionKind.Work => _localizer.GetText(LocalizedText.TimerWorkCompleted),
        TimerStatus.Completed when _timerService.LastCompletionKind == TimerCompletionKind.Break => _localizer.GetText(LocalizedText.TimerBreakCompleted),
        _ => _localizer.GetText(LocalizedText.TimerReady),
    };

    public string RemainingTimeText => FormatDuration(_timerService.RemainingTime);

    public double Progress => _timerService.Progress;

    public double ProgressPercent => _timerService.Progress * 100;

    public string ProgressText => _localizer.Format(LocalizedText.TimerProgressFormat, ProgressPercent);

    public string BreakHintText => _localizer.Format(LocalizedText.TimerBreakHintFormat, _timerService.RemainingTime == TimeSpan.Zero ? 0 : BreakDurationMinutes);

    public string TopicLabel => _localizer.GetText(LocalizedText.TimerTopic);

    public string TopicPlaceholder => _localizer.GetText(LocalizedText.TimerTopicPlaceholder);

    public string StartButtonText
    {
        get
        {
            if (_timerService.Status == TimerStatus.Completed && _timerService.LastCompletionKind == TimerCompletionKind.Break)
            {
                return _localizer.GetText(LocalizedText.TimerStartNext);
            }

            return _timerService.Mode == TimerMode.Break
                ? _localizer.GetText(LocalizedText.TimerStartBreak)
                : _localizer.GetText(LocalizedText.TimerStart);
        }
    }

    public string PauseButtonText => _localizer.GetText(LocalizedText.TimerPause);

    public string ResumeButtonText => _localizer.GetText(LocalizedText.TimerResume);

    public string ResetButtonText => _localizer.GetText(LocalizedText.TimerReset);

    public string TodayPomodorosLabel => _localizer.GetText(LocalizedText.StatsTodayPomodoros);

    public string FocusMinutesLabel => _localizer.GetText(LocalizedText.StatsFocusMinutes);

    public string TodayTasksLabel => _localizer.GetText(LocalizedText.TimerTodayTasks);

    public string NoSessionsText => _localizer.GetText(LocalizedText.TimerNoSessions);

    public string TopicHeader => _localizer.GetText(LocalizedText.TimerTaskTopicHeader);

    public string CompletedTimeHeader => _localizer.GetText(LocalizedText.TimerTaskCompletedTimeHeader);

    public string DurationHeader => _localizer.GetText(LocalizedText.TimerTaskDurationHeader);

    public string StatusHeader => _localizer.GetText(LocalizedText.TimerTaskStatusHeader);

    public bool ShowStartButton => _timerService.Status is TimerStatus.Idle or TimerStatus.Completed;

    public bool ShowPauseButton => _timerService.Status == TimerStatus.Running;

    public bool ShowResumeButton => _timerService.Status == TimerStatus.Paused;

    public bool CanEditTopic => _timerService.Status != TimerStatus.Running || _timerService.Mode != TimerMode.Work;

    public bool HasSessions => Sessions.Count > 0;

    public bool ShowEmptyState => !HasSessions;

    public int BreakDurationMinutes => (int)_timerService.BreakDuration.TotalMinutes;

    public async Task RefreshTodayAsync()
    {
        _currentDate = DateTime.Now.Date;
        var sessions = await _sessionStore.GetSessionsByDateAsync(_currentDate).ConfigureAwait(true);
        var completed = sessions.Where(session => session.Completed).ToList();

        Sessions.Clear();
        foreach (var session in sessions)
        {
            Sessions.Add(CreateRow(session));
        }

        TodayPomodoros = completed.Count;
        TodayFocusMinutes = completed.Sum(session => session.PlannedMinutes);
        OnPropertyChanged(nameof(HasSessions));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    public void UpdateSettings(AppSettings settings)
    {
        _timerService.UpdateSettings(settings);
        RaiseTimerStateProperties();
    }

    private void Start()
    {
        _timerService.Start(CurrentTopic, _localizer.GetText(LocalizedText.TaskUntitled));
        RaiseTimerStateProperties();
    }

    private void Pause()
    {
        _timerService.Pause();
        RaiseTimerStateProperties();
    }

    private void Resume()
    {
        _timerService.Resume();
        RaiseTimerStateProperties();
    }

    private void Reset()
    {
        _timerService.Reset();
        RaiseTimerStateProperties();
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        if (DateTime.Now.Date != _currentDate)
        {
            await RefreshTodayAsync().ConfigureAwait(true);
            SessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        var result = _timerService.Tick();
        if (result.CompletedSession is not null)
        {
            await _sessionStore.SaveSessionAsync(result.CompletedSession).ConfigureAwait(true);
            await RefreshTodayAsync().ConfigureAwait(true);
            SessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        RaiseTimerStateProperties();
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
        _ = RefreshTodayAsync();
    }

    private void RaiseTimerStateProperties()
    {
        OnPropertyChanged(nameof(ModeText));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(RemainingTimeText));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressPercent));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(BreakHintText));
        OnPropertyChanged(nameof(BreakDurationMinutes));
        OnPropertyChanged(nameof(StartButtonText));
        OnPropertyChanged(nameof(ShowStartButton));
        OnPropertyChanged(nameof(ShowPauseButton));
        OnPropertyChanged(nameof(ShowResumeButton));
        OnPropertyChanged(nameof(CanEditTopic));
    }

    private void RaiseLocalizedProperties()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(ModeText));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(BreakHintText));
        OnPropertyChanged(nameof(TopicLabel));
        OnPropertyChanged(nameof(TopicPlaceholder));
        OnPropertyChanged(nameof(StartButtonText));
        OnPropertyChanged(nameof(PauseButtonText));
        OnPropertyChanged(nameof(ResumeButtonText));
        OnPropertyChanged(nameof(ResetButtonText));
        OnPropertyChanged(nameof(TodayPomodorosLabel));
        OnPropertyChanged(nameof(FocusMinutesLabel));
        OnPropertyChanged(nameof(TodayTasksLabel));
        OnPropertyChanged(nameof(NoSessionsText));
        OnPropertyChanged(nameof(TopicHeader));
        OnPropertyChanged(nameof(CompletedTimeHeader));
        OnPropertyChanged(nameof(DurationHeader));
        OnPropertyChanged(nameof(StatusHeader));
    }

    private static string FormatDuration(TimeSpan duration)
    {
        var totalSeconds = Math.Max(0, (int)Math.Ceiling(duration.TotalSeconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }
}
