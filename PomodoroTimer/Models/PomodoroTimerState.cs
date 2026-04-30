using System;

namespace PomodoroTimer.Models;

public sealed class PomodoroTimerState
{
    public TimerMode Mode { get; init; } = TimerMode.Work;

    public TimerStatus Status { get; init; } = TimerStatus.Idle;

    public TimeSpan RemainingTime { get; init; } = TimeSpan.FromMinutes(AppSettings.DefaultWorkDurationMinutes);

    public TimeSpan CurrentDuration { get; init; } = TimeSpan.FromMinutes(AppSettings.DefaultWorkDurationMinutes);

    public double Progress { get; init; }
}
