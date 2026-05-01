using System;

namespace PomodoroTimer.Models;

public sealed class TodayTask
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public bool Completed { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public int CompletedPomodoros { get; set; }
}
