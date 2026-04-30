using System;

namespace PomodoroTimer.Models;

public sealed class FocusSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Topic { get; set; } = string.Empty;

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset EndedAt { get; set; }

    public int PlannedMinutes { get; set; }

    public int ActualMinutes { get; set; }

    public bool Completed { get; set; }
}
