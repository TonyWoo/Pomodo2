using System;

namespace PomodoroTimer.Models;

public sealed class PomodoroTimerState
{
    private readonly TimeSpan _focusDuration;
    private readonly TimeSpan _breakDuration;

    public PomodoroTimerState(TimeSpan? focusDuration = null, TimeSpan? breakDuration = null)
    {
        _focusDuration = focusDuration ?? TimeSpan.FromMinutes(25);
        _breakDuration = breakDuration ?? TimeSpan.FromMinutes(5);

        if (_focusDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(focusDuration), "Focus duration must be positive.");
        }

        if (_breakDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(breakDuration), "Break duration must be positive.");
        }

        IsFocusSession = true;
        TimeRemaining = _focusDuration;
        Status = PomodoroTimerStatus.ReadyToStart;
    }

    public bool IsFocusSession { get; private set; }

    public bool IsRunning { get; private set; }

    public int CompletedFocusSessions { get; private set; }

    public TimeSpan TimeRemaining { get; private set; }

    public PomodoroTimerStatus Status { get; private set; }

    public TimeSpan CurrentDuration => IsFocusSession ? _focusDuration : _breakDuration;

    public void ToggleRunning()
    {
        IsRunning = !IsRunning;
        Status = IsRunning
            ? IsFocusSession
                ? PomodoroTimerStatus.FocusRunning
                : PomodoroTimerStatus.BreakRunning
            : IsFocusSession
                ? PomodoroTimerStatus.FocusPaused
                : PomodoroTimerStatus.BreakPaused;
    }

    public void Reset()
    {
        IsRunning = false;
        IsFocusSession = true;
        CompletedFocusSessions = 0;
        TimeRemaining = _focusDuration;
        Status = PomodoroTimerStatus.Reset;
    }

    public void SkipPhase()
    {
        CompleteCurrentPhase(manualAdvance: true);
    }

    public void Advance(TimeSpan elapsed)
    {
        if (!IsRunning || elapsed <= TimeSpan.Zero)
        {
            return;
        }

        if (elapsed < TimeRemaining)
        {
            TimeRemaining -= elapsed;
            return;
        }

        CompleteCurrentPhase(manualAdvance: false);
    }

    private void CompleteCurrentPhase(bool manualAdvance)
    {
        IsRunning = false;

        if (IsFocusSession)
        {
            if (!manualAdvance)
            {
                CompletedFocusSessions++;
            }

            IsFocusSession = false;
            TimeRemaining = _breakDuration;
            Status = manualAdvance
                ? PomodoroTimerStatus.SwitchedToBreak
                : PomodoroTimerStatus.FocusCompleted;
            return;
        }

        IsFocusSession = true;
        TimeRemaining = _focusDuration;
        Status = manualAdvance
            ? PomodoroTimerStatus.SwitchedToFocus
            : PomodoroTimerStatus.BreakCompleted;
    }
}
