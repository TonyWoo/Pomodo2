using System;

namespace PomodoroTimer.Models;

public sealed class DailyStats
{
    public DateOnly Date { get; set; }

    public int CompletedPomodoros { get; set; }

    public int TotalFocusMinutes { get; set; }
}
