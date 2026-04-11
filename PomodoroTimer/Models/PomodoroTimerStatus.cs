namespace PomodoroTimer.Models;

public enum PomodoroTimerStatus
{
    ReadyToStart,
    FocusRunning,
    BreakRunning,
    FocusPaused,
    BreakPaused,
    Reset,
    SwitchedToBreak,
    FocusCompleted,
    SwitchedToFocus,
    BreakCompleted,
}
