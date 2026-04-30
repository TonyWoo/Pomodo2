using System;
using System.Collections.Generic;
using System.Linq;

namespace PomodoroTimer.Models;

public sealed class DailyStats
{
    public DateOnly Date { get; set; }

    public int CompletedPomodoros { get; set; }

    public int TotalFocusMinutes { get; set; }

    public static DailyStats FromSessions(DateOnly date, IEnumerable<FocusSession> sessions)
    {
        var completed = sessions
            .Where(session => session.Completed && DateOnly.FromDateTime(session.EndedAt.LocalDateTime) == date)
            .ToList();

        return new DailyStats
        {
            Date = date,
            CompletedPomodoros = completed.Count,
            TotalFocusMinutes = completed.Sum(session => session.PlannedMinutes),
        };
    }
}
