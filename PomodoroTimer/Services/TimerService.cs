using System;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class TimerService
{
    private readonly IClock _clock;
    private TimeSpan _workDuration;
    private TimeSpan _breakDuration;
    private DateTimeOffset? _targetEndTime;
    private DateTimeOffset? _currentSessionStartedAt;
    private string _currentTopic = string.Empty;

    public TimerService(AppSettings settings, IClock? clock = null)
    {
        _clock = clock ?? new SystemClock();
        ApplyDurations(settings);
        Mode = TimerMode.Work;
        Status = TimerStatus.Idle;
        RemainingTime = _workDuration;
    }

    public TimerMode Mode { get; private set; }

    public TimerStatus Status { get; private set; }

    public TimerCompletionKind LastCompletionKind { get; private set; } = TimerCompletionKind.None;

    public TimeSpan RemainingTime { get; private set; }

    public TimeSpan WorkDuration => _workDuration;

    public TimeSpan BreakDuration => _breakDuration;

    public TimeSpan CurrentDuration => Mode == TimerMode.Work ? _workDuration : _breakDuration;

    public double Progress
    {
        get
        {
            var totalSeconds = CurrentDuration.TotalSeconds;
            if (totalSeconds <= 0)
            {
                return 0;
            }

            var elapsed = CurrentDuration - RemainingTime;
            return Math.Clamp(elapsed.TotalSeconds / totalSeconds, 0, 1);
        }
    }

    public bool IsRunning => Status == TimerStatus.Running;

    public void Start(string topic, string defaultTopic)
    {
        if (Status == TimerStatus.Running)
        {
            return;
        }

        if (Status == TimerStatus.Paused)
        {
            Resume();
            return;
        }

        if (Mode == TimerMode.Work)
        {
            _currentTopic = string.IsNullOrWhiteSpace(topic)
                ? defaultTopic
                : topic.Trim();
            _currentSessionStartedAt = _clock.UtcNow;
        }

        LastCompletionKind = TimerCompletionKind.None;
        _targetEndTime = _clock.UtcNow.Add(RemainingTime);
        Status = TimerStatus.Running;
    }

    public void Resume()
    {
        if (Status != TimerStatus.Paused)
        {
            return;
        }

        LastCompletionKind = TimerCompletionKind.None;
        _targetEndTime = _clock.UtcNow.Add(RemainingTime);
        Status = TimerStatus.Running;
    }

    public void Pause()
    {
        if (Status != TimerStatus.Running)
        {
            return;
        }

        RefreshRemainingTime();
        _targetEndTime = null;
        Status = TimerStatus.Paused;
    }

    public void Reset()
    {
        Mode = TimerMode.Work;
        Status = TimerStatus.Idle;
        LastCompletionKind = TimerCompletionKind.None;
        RemainingTime = _workDuration;
        _targetEndTime = null;
        _currentSessionStartedAt = null;
        _currentTopic = string.Empty;
    }

    public TimerTickResult Tick()
    {
        if (Status != TimerStatus.Running)
        {
            return TimerTickResult.None;
        }

        RefreshRemainingTime();
        return RemainingTime == TimeSpan.Zero
            ? CompleteCurrentMode()
            : TimerTickResult.None;
    }

    public void UpdateSettings(AppSettings settings)
    {
        ApplyDurations(settings);

        if (Status is TimerStatus.Idle or TimerStatus.Completed)
        {
            RemainingTime = CurrentDuration;
        }
    }

    private void ApplyDurations(AppSettings settings)
    {
        var workMinutes = Math.Clamp(settings.WorkDurationMinutes, 1, 180);
        var breakMinutes = Math.Clamp(settings.BreakDurationMinutes, 1, 60);
        _workDuration = TimeSpan.FromMinutes(workMinutes);
        _breakDuration = TimeSpan.FromMinutes(breakMinutes);
    }

    private void RefreshRemainingTime()
    {
        if (_targetEndTime is null)
        {
            return;
        }

        var remaining = _targetEndTime.Value - _clock.UtcNow;
        RemainingTime = remaining <= TimeSpan.Zero
            ? TimeSpan.Zero
            : remaining;
    }

    private TimerTickResult CompleteCurrentMode()
    {
        var completionTime = _clock.UtcNow;
        _targetEndTime = null;
        Status = TimerStatus.Completed;

        if (Mode == TimerMode.Work)
        {
            var startedAt = _currentSessionStartedAt ?? completionTime.Subtract(_workDuration);
            var actualMinutes = Math.Max(
                1,
                (int)Math.Round((completionTime - startedAt).TotalMinutes, MidpointRounding.AwayFromZero));

            var session = new FocusSession
            {
                Topic = _currentTopic,
                StartedAt = startedAt,
                EndedAt = completionTime,
                PlannedMinutes = (int)_workDuration.TotalMinutes,
                ActualMinutes = actualMinutes,
                Completed = true,
            };

            Mode = TimerMode.Break;
            LastCompletionKind = TimerCompletionKind.Work;
            RemainingTime = _breakDuration;
            _currentSessionStartedAt = null;
            _currentTopic = string.Empty;

            return new TimerTickResult
            {
                CompletedSession = session,
                CompletionKind = TimerCompletionKind.Work,
            };
        }

        Mode = TimerMode.Work;
        LastCompletionKind = TimerCompletionKind.Break;
        RemainingTime = _workDuration;

        return new TimerTickResult
        {
            CompletionKind = TimerCompletionKind.Break,
        };
    }
}
