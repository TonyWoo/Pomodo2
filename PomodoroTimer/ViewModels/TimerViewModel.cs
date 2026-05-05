using System;
using System.Collections.Generic;
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
    private static readonly TimeSpan UiTickInterval = TimeSpan.FromMilliseconds(250);
    private readonly AppLocalizer _localizer;
    private readonly ISessionStore _sessionStore;
    private readonly ITaskStore _taskStore;
    private readonly TimerService _timerService;
    private readonly List<FocusSession> _sessions;
    private readonly List<TodayTask> _tasks;
    private readonly DispatcherTimer _uiTimer;
    private string _topic = string.Empty;
    private bool _isCompactLayout;
    private Guid? _activeTaskId;

    /// <summary>
    /// Set by the view to provide a platform confirm dialog.
    /// Receives (title, message) and returns true if the user confirmed.
    /// </summary>
    public Func<string, string, Task<bool>>? ConfirmDeleteTask { get; set; }

    public TimerViewModel(
        TimerService timerService,
        AppLocalizer localizer,
        ISessionStore sessionStore,
        ITaskStore taskStore,
        IEnumerable<FocusSession> sessions,
        IEnumerable<TodayTask> tasks)
    {
        _timerService = timerService;
        _localizer = localizer;
        _sessionStore = sessionStore;
        _taskStore = taskStore;
        _sessions = sessions.OrderBy(session => session.StartedAt).ToList();
        _tasks = tasks.OrderBy(task => task.CreatedAt).ToList();
        TodaySessions = [];
        TodayTasks = [];

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
        AddTaskCommand = new RelayCommand(AddTask, CanAddTask);

        RefreshTodaySessions();
        RefreshTodayTasks();
    }

    public event EventHandler<IReadOnlyList<FocusSession>>? SessionsChanged;

    public IRelayCommand StartCommand { get; }

    public IRelayCommand PauseCommand { get; }

    public IRelayCommand ResumeCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public IRelayCommand PrimaryCommand { get; }

    public IRelayCommand AddTaskCommand { get; }

    public ObservableCollection<SessionListItemViewModel> TodaySessions { get; }

    public ObservableCollection<TaskListItemViewModel> TodayTasks { get; }

    public IReadOnlyList<FocusSession> AllSessions => _sessions;

    public string Topic
    {
        get => _topic;
        set
        {
            if (SetProperty(ref _topic, value))
            {
                AddTaskCommand.NotifyCanExecuteChanged();
            }
        }
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

    public bool IsRunning => _timerService.Status == TimerStatus.Running;

    public bool IsNotRunning => !IsRunning;

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

    public string StartActionText
    {
        get
        {
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

    public string PauseText => _localizer.GetText(LocalizedText.TimerPause);

    public string TodayPomodorosLabel => _localizer.GetText(LocalizedText.StatsTodayPomodoros);

    public string TodayTasksLabel => _localizer.GetText(LocalizedText.TimerTodayTasks);

    public string ViewAllText => _localizer.GetText(LocalizedText.TimerViewAll);

    public string EmptySessionsText => _localizer.GetText(LocalizedText.TimerNoSessions);

    public string EmptyTasksText => _localizer.GetText(LocalizedText.TaskNoItems);

    public string AddTaskText => _localizer.GetText(LocalizedText.TaskAdd);

    public string SessionTopicHeader => _localizer.GetText(LocalizedText.SessionTopic);

    public string SessionCompletedAtHeader => _localizer.GetText(LocalizedText.SessionCompletedAt);

    public string SessionDurationHeader => _localizer.GetText(LocalizedText.SessionDuration);

    public string SessionStatusHeader => _localizer.GetText(LocalizedText.SessionStatus);

    public int TodayPomodoros => TodayCompletedSessions.Count;

    public string TodayPomodorosText => TodayPomodoros.ToString(CultureInfo.InvariantCulture);

    public bool HasTodaySessions => TodaySessions.Count > 0;

    public bool HasNoTodaySessions => !HasTodaySessions;

    public bool HasTodayTasks => TodayTasks.Count > 0;

    public bool HasNoTodayTasks => !HasTodayTasks;

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
        OnPropertyChanged(nameof(StartActionText));
        OnPropertyChanged(nameof(ResetText));
        OnPropertyChanged(nameof(PauseText));
        OnPropertyChanged(nameof(TodayPomodorosLabel));
        OnPropertyChanged(nameof(TodayTasksLabel));
        OnPropertyChanged(nameof(ViewAllText));
        OnPropertyChanged(nameof(EmptySessionsText));
        OnPropertyChanged(nameof(EmptyTasksText));
        OnPropertyChanged(nameof(AddTaskText));
        OnPropertyChanged(nameof(SessionTopicHeader));
        OnPropertyChanged(nameof(SessionCompletedAtHeader));
        OnPropertyChanged(nameof(SessionDurationHeader));
        OnPropertyChanged(nameof(SessionStatusHeader));
        foreach (var task in TodayTasks)
        {
            task.Refresh();
        }
    }

    private bool CanStart()
    {
        return _timerService.Status != TimerStatus.Running;
    }

    private bool CanAddTask()
    {
        return !string.IsNullOrWhiteSpace(Topic);
    }

    private void Start()
    {
        if (_timerService.Mode == TimerMode.Work
            && _timerService.Status is TimerStatus.Idle or TimerStatus.Completed)
        {
            var task = EnsureTaskForStart();
            if (task is not null)
            {
                _activeTaskId = task.Id;
                Topic = task.Title;
            }
        }

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
        await IncrementTaskPomodoroAsync(session).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshTodaySessions();
            RefreshTodayTasks();
            RaiseTimerPropertiesChanged();
            SessionsChanged?.Invoke(this, AllSessions);
        });
    }

    private void AddTask()
    {
        var task = EnsureTaskForStart(forceCreate: true);
        if (task is null)
        {
            return;
        }

        _activeTaskId = task.Id;
        Topic = task.Title;
        RefreshTodayTasks();
    }

    private TodayTask? EnsureTaskForStart(bool forceCreate = false)
    {
        var normalizedTopic = Topic.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTopic))
        {
            return _activeTaskId is null ? null : TodayActiveTasks.SingleOrDefault(task => task.Id == _activeTaskId.Value);
        }

        var existingTask = TodayActiveTasks
            .FirstOrDefault(task => string.Equals(task.Title, normalizedTopic, StringComparison.OrdinalIgnoreCase));
        if (existingTask is not null)
        {
            return existingTask;
        }

        if (!forceCreate && _timerService.Mode != TimerMode.Work)
        {
            return null;
        }

        var task = new TodayTask
        {
            Title = normalizedTopic,
            CreatedAt = DateTimeOffset.Now,
        };

        _tasks.Add(task);
        _ = _taskStore.SaveTaskAsync(task);
        RefreshTodayTasks();
        return task;
    }

    private async Task IncrementTaskPomodoroAsync(FocusSession session)
    {
        var task = ResolveTaskForCompletedSession(session);
        if (task is null)
        {
            return;
        }

        task.CompletedPomodoros += 1;
        await _taskStore.SaveTaskAsync(task).ConfigureAwait(false);
    }

    private TodayTask? ResolveTaskForCompletedSession(FocusSession session)
    {
        if (_activeTaskId is not null)
        {
            var activeTask = TodayActiveTasks.SingleOrDefault(task => task.Id == _activeTaskId.Value);
            if (activeTask is not null)
            {
                return activeTask;
            }
        }

        return TodayActiveTasks.FirstOrDefault(task =>
            string.Equals(task.Title, session.Topic, StringComparison.OrdinalIgnoreCase));
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

    private IReadOnlyList<TodayTask> TodayActiveTasks =>
        _tasks
            .Where(task => DateOnly.FromDateTime(task.CreatedAt.LocalDateTime) == DateOnly.FromDateTime(DateTime.Now))
            .OrderBy(task => task.Completed)
            .ThenByDescending(task => task.CreatedAt)
            .ToList();

    private void RefreshTodayTasks()
    {
        TodayTasks.Clear();
        foreach (var task in TodayActiveTasks)
        {
            TodayTasks.Add(new TaskListItemViewModel(
                task,
                _localizer,
                UseTask,
                ToggleTaskCompleted,
                DeleteTask,
                ConfirmDeleteTask));
        }

        foreach (var taskItem in TodayTasks)
        {
            taskItem.IsActive = _activeTaskId == taskItem.Id;
        }

        OnPropertyChanged(nameof(HasTodayTasks));
        OnPropertyChanged(nameof(HasNoTodayTasks));
    }

    private void UseTask(TodayTask task)
    {
        _activeTaskId = task.Id;
        Topic = task.Title;
        RefreshTodayTasks();
    }

    private async void ToggleTaskCompleted(TodayTask task)
    {
        task.Completed = !task.Completed;
        task.CompletedAt = task.Completed ? DateTimeOffset.Now : null;
        await _taskStore.SaveTaskAsync(task).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshTodayTasks();
        });
    }

    private async void DeleteTask(TodayTask task)
    {
        _tasks.RemoveAll(existing => existing.Id == task.Id);
        if (_activeTaskId == task.Id)
        {
            _activeTaskId = null;
        }

        await _taskStore.DeleteTaskAsync(task.Id).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RefreshTodayTasks();
        });
    }

    private void RaiseTimerPropertiesChanged()
    {
        OnPropertyChanged(nameof(RemainingTimeText));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(IsNotRunning));
        OnPropertyChanged(nameof(ModeText));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(BreakHintText));
        OnPropertyChanged(nameof(StartActionText));
        OnPropertyChanged(nameof(PrimaryActionText));
        OnPropertyChanged(nameof(TodayPomodoros));
        OnPropertyChanged(nameof(TodayPomodorosText));
        StartCommand.NotifyCanExecuteChanged();
        PauseCommand.NotifyCanExecuteChanged();
        ResumeCommand.NotifyCanExecuteChanged();
        PrimaryCommand.NotifyCanExecuteChanged();
        AddTaskCommand.NotifyCanExecuteChanged();
    }
}
