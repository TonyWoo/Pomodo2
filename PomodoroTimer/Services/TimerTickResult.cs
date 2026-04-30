using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class TimerTickResult
{
    public static TimerTickResult None { get; } = new();

    public FocusSession? CompletedSession { get; init; }

    public TimerCompletionKind CompletionKind { get; init; } = TimerCompletionKind.None;
}
