using System;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class TimerService
{
    private readonly IClock _clock;
    private DateTimeOffset? _targetEndTime;
    private DateTimeOffset? _startedAt;
    private string _currentTopic = string.Empty;

    public TimerService(AppSettings settings, IClock? clock = null)
    {
        _clock = clock ?? new SystemClock();
        Settings = settings;
        Settings.Normalize();
        Mode = TimerMode.Work;
        Status = TimerStatus.Idle;
        RemainingTime = WorkDuration;
    }

    public event EventHandler<FocusSession>? WorkSessionCompleted;

    public AppSettings Settings { get; }

    public TimerMode Mode { get; private set; }

    public TimerStatus Status { get; private set; }

    public TimeSpan RemainingTime { get; private set; }

    public TimeSpan CurrentDuration => Mode == TimerMode.Work ? WorkDuration : BreakDuration;

    public double Progress
    {
        get
        {
            if (CurrentDuration <= TimeSpan.Zero)
            {
                return 0;
            }

            var elapsed = CurrentDuration - RemainingTime;
            return Math.Clamp(elapsed.TotalSeconds / CurrentDuration.TotalSeconds, 0, 1);
        }
    }

    private TimeSpan WorkDuration => TimeSpan.FromMinutes(Settings.WorkDurationMinutes);

    private TimeSpan BreakDuration => TimeSpan.FromMinutes(Settings.BreakDurationMinutes);

    public void Start(string topic, string untitledTopic)
    {
        if (Status == TimerStatus.Running)
        {
            return;
        }

        if (Status is TimerStatus.Idle or TimerStatus.Completed)
        {
            Status = TimerStatus.Idle;
            if (Mode == TimerMode.Work)
            {
                _startedAt = _clock.Now;
                _currentTopic = string.IsNullOrWhiteSpace(topic) ? untitledTopic : topic.Trim();
            }
        }

        _targetEndTime = _clock.Now + RemainingTime;
        Status = TimerStatus.Running;
    }

    public void Pause()
    {
        if (Status != TimerStatus.Running)
        {
            return;
        }

        UpdateRemainingFromClock();
        _targetEndTime = null;
        Status = TimerStatus.Paused;
    }

    public void Reset()
    {
        _targetEndTime = null;
        _startedAt = null;
        _currentTopic = string.Empty;
        Mode = TimerMode.Work;
        Status = TimerStatus.Idle;
        RemainingTime = WorkDuration;
    }

    public void Tick()
    {
        if (Status != TimerStatus.Running)
        {
            return;
        }

        UpdateRemainingFromClock();
        if (RemainingTime > TimeSpan.Zero)
        {
            return;
        }

        CompleteCurrentMode();
    }

    public void ApplySettings()
    {
        Settings.Normalize();
        if (Status != TimerStatus.Idle)
        {
            return;
        }

        RemainingTime = CurrentDuration;
    }

    private void UpdateRemainingFromClock()
    {
        if (_targetEndTime is null)
        {
            return;
        }

        RemainingTime = _targetEndTime.Value - _clock.Now;
        if (RemainingTime < TimeSpan.Zero)
        {
            RemainingTime = TimeSpan.Zero;
        }
    }

    private void CompleteCurrentMode()
    {
        _targetEndTime = null;
        Status = TimerStatus.Completed;

        if (Mode == TimerMode.Work)
        {
            var endedAt = _clock.Now;
            var startedAt = _startedAt ?? endedAt - WorkDuration;
            var session = new FocusSession
            {
                Topic = _currentTopic,
                StartedAt = startedAt,
                EndedAt = endedAt,
                PlannedMinutes = Settings.WorkDurationMinutes,
                ActualMinutes = Math.Max(1, (int)Math.Round((endedAt - startedAt).TotalMinutes)),
                Completed = true,
            };

            WorkSessionCompleted?.Invoke(this, session);

            Mode = TimerMode.Break;
            RemainingTime = BreakDuration;
            _startedAt = null;
            _currentTopic = string.Empty;
            Status = Settings.AutoStartBreak ? TimerStatus.Running : TimerStatus.Idle;
            if (Settings.AutoStartBreak)
            {
                _targetEndTime = _clock.Now + RemainingTime;
            }

            return;
        }

        Mode = TimerMode.Work;
        RemainingTime = WorkDuration;
        Status = Settings.AutoStartNextWork ? TimerStatus.Running : TimerStatus.Idle;
        if (Settings.AutoStartNextWork)
        {
            _startedAt = _clock.Now;
            _targetEndTime = _clock.Now + RemainingTime;
        }
    }
}
