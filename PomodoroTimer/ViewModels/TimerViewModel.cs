using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class TimerViewModel : ViewModelBase
{
    private static readonly TimeSpan UiTickInterval = TimeSpan.FromMilliseconds(250);
    private readonly AppLocalizer _localizer;
    private readonly ISessionStore _sessionStore;
    private readonly TimerService _timerService;
    private readonly List<FocusSession> _sessions;
    private readonly DispatcherTimer _uiTimer;
    private string _topic = string.Empty;
    private bool _isCompactLayout;

    public TimerViewModel(
        TimerService timerService,
        AppLocalizer localizer,
        ISessionStore sessionStore,
        IEnumerable<FocusSession> sessions)
    {
        _timerService = timerService;
        _localizer = localizer;
        _sessionStore = sessionStore;
        _sessions = sessions.OrderBy(session => session.StartedAt).ToList();
        TodaySessions = [];

        _timerService.WorkSessionCompleted += OnWorkSessionCompleted;

        _uiTimer = new DispatcherTimer { Interval = UiTickInterval };
        _uiTimer.Tick += (_, _) =>
        {
            _timerService.Tick();
            if (_timerService.Status != TimerStatus.Running)
            {
                _uiTimer.Stop();
            }

            RaiseTimerPropertiesChanged();
        };

        StartCommand = new RelayCommand(Start, CanStart);
        PauseCommand = new RelayCommand(Pause, () => _timerService.Status == TimerStatus.Running);
        ResumeCommand = new RelayCommand(Start, () => _timerService.Status == TimerStatus.Paused);
        ResetCommand = new RelayCommand(Reset);
        PrimaryCommand = new RelayCommand(RunPrimaryAction);

        RefreshTodaySessions();
    }

    public event EventHandler<IReadOnlyList<FocusSession>>? SessionsChanged;

    public IRelayCommand StartCommand { get; }

    public IRelayCommand PauseCommand { get; }

    public IRelayCommand ResumeCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public IRelayCommand PrimaryCommand { get; }

    public ObservableCollection<SessionListItemViewModel> TodaySessions { get; }

    public IReadOnlyList<FocusSession> AllSessions => _sessions;

    public string Topic
    {
        get => _topic;
        set => SetProperty(ref _topic, value);
    }

    public bool IsCompactLayout
    {
        get => _isCompactLayout;
        set
        {
            if (SetProperty(ref _isCompactLayout, value))
            {
                OnPropertyChanged(nameof(IsWideLayout));
            }
        }
    }

    public bool IsWideLayout => !IsCompactLayout;

    public string RemainingTimeText => _timerService.RemainingTime.ToString(@"mm\:ss", CultureInfo.InvariantCulture);

    public double Progress => _timerService.Progress;

    public string ModeText => _timerService.Mode == TimerMode.Work
        ? _localizer.GetText(LocalizedText.TimerWork)
        : _localizer.GetText(LocalizedText.TimerBreak);

    public string StatusText => _timerService.Status switch
    {
        TimerStatus.Running => _localizer.GetText(LocalizedText.TimerRunning),
        TimerStatus.Paused => _localizer.GetText(LocalizedText.TimerPaused),
        _ => _localizer.GetText(LocalizedText.TimerReady),
    };

    public string BreakHintText => _localizer.Format(LocalizedText.TimerBreakHintFormat, _timerService.Settings.BreakDurationMinutes);

    public string TopicLabel => _localizer.GetText(LocalizedText.TimerTopic);

    public string TopicPlaceholder => _localizer.GetText(LocalizedText.TimerTopicPlaceholder);

    public string PrimaryActionText
    {
        get
        {
            if (_timerService.Status == TimerStatus.Running)
            {
                return _localizer.GetText(LocalizedText.TimerPause);
            }

            if (_timerService.Status == TimerStatus.Paused)
            {
                return _localizer.GetText(LocalizedText.TimerResume);
            }

            if (_timerService.Mode == TimerMode.Break)
            {
                return _localizer.GetText(LocalizedText.TimerStartBreak);
            }

            return _localizer.GetText(LocalizedText.TimerStart);
        }
    }

    public string ResetText => _localizer.GetText(LocalizedText.TimerReset);

    public string TodayPomodorosLabel => _localizer.GetText(LocalizedText.StatsTodayPomodoros);

    public string TodayTasksLabel => _localizer.GetText(LocalizedText.TimerTodayTasks);

    public string ViewAllText => _localizer.GetText(LocalizedText.TimerViewAll);

    public string EmptySessionsText => _localizer.GetText(LocalizedText.TimerNoSessions);

    public string SessionTopicHeader => _localizer.GetText(LocalizedText.SessionTopic);

    public string SessionCompletedAtHeader => _localizer.GetText(LocalizedText.SessionCompletedAt);

    public string SessionDurationHeader => _localizer.GetText(LocalizedText.SessionDuration);

    public string SessionStatusHeader => _localizer.GetText(LocalizedText.SessionStatus);

    public int TodayPomodoros => TodayCompletedSessions.Count;

    public string TodayPomodorosText => TodayPomodoros.ToString(CultureInfo.InvariantCulture);

    public bool HasTodaySessions => TodaySessions.Count > 0;

    public bool HasNoTodaySessions => !HasTodaySessions;

    private IReadOnlyList<FocusSession> TodayCompletedSessions
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return _sessions
                .Where(session => session.Completed && DateOnly.FromDateTime(session.EndedAt.LocalDateTime) == today)
                .OrderByDescending(session => session.EndedAt)
                .ToList();
        }
    }

    public void ApplySettings()
    {
        _timerService.ApplySettings();
        RaiseTimerPropertiesChanged();
    }

    public void RefreshLocalization()
    {
        RefreshTodaySessions();
        RaiseTimerPropertiesChanged();
        OnPropertyChanged(nameof(TopicLabel));
        OnPropertyChanged(nameof(TopicPlaceholder));
        OnPropertyChanged(nameof(ResetText));
        OnPropertyChanged(nameof(TodayPomodorosLabel));
        OnPropertyChanged(nameof(TodayTasksLabel));
        OnPropertyChanged(nameof(ViewAllText));
        OnPropertyChanged(nameof(EmptySessionsText));
        OnPropertyChanged(nameof(SessionTopicHeader));
        OnPropertyChanged(nameof(SessionCompletedAtHeader));
        OnPropertyChanged(nameof(SessionDurationHeader));
        OnPropertyChanged(nameof(SessionStatusHeader));
    }

    private bool CanStart()
    {
        return _timerService.Status != TimerStatus.Running;
    }

    private void Start()
    {
        _timerService.Start(Topic, _localizer.GetText(LocalizedText.TaskUntitled));
        _uiTimer.Start();
        RaiseTimerPropertiesChanged();
    }

    private void Pause()
    {
        _timerService.Pause();
        _uiTimer.Stop();
        RaiseTimerPropertiesChanged();
    }

    private void Reset()
    {
        _timerService.Reset();
        _uiTimer.Stop();
        RaiseTimerPropertiesChanged();
    }

    private void RunPrimaryAction()
    {
        if (_timerService.Status == TimerStatus.Running)
        {
            Pause();
            return;
        }

        Start();
    }

    private async void OnWorkSessionCompleted(object? sender, FocusSession session)
    {
        _sessions.Add(session);
        await _sessionStore.SaveSessionAsync(session).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshTodaySessions();
            RaiseTimerPropertiesChanged();
            SessionsChanged?.Invoke(this, AllSessions);
        });
    }

    private void RefreshTodaySessions()
    {
        TodaySessions.Clear();
        foreach (var session in TodayCompletedSessions)
        {
            TodaySessions.Add(SessionListItemViewModel.FromSession(session, _localizer));
        }

        OnPropertyChanged(nameof(TodayPomodoros));
        OnPropertyChanged(nameof(TodayPomodorosText));
        OnPropertyChanged(nameof(HasTodaySessions));
        OnPropertyChanged(nameof(HasNoTodaySessions));
    }

    private void RaiseTimerPropertiesChanged()
    {
        OnPropertyChanged(nameof(RemainingTimeText));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ModeText));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(BreakHintText));
        OnPropertyChanged(nameof(PrimaryActionText));
        OnPropertyChanged(nameof(TodayPomodoros));
        OnPropertyChanged(nameof(TodayPomodorosText));
        StartCommand.NotifyCanExecuteChanged();
        PauseCommand.NotifyCanExecuteChanged();
        ResumeCommand.NotifyCanExecuteChanged();
        PrimaryCommand.NotifyCanExecuteChanged();
    }
}
