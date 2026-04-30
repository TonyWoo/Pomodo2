using System;

namespace PomodoroTimer.Models;

public sealed class AppSettings
{
    public const int DefaultWorkDurationMinutes = 25;
    public const int DefaultBreakDurationMinutes = 5;
    public const int MinimumWorkDurationMinutes = 1;
    public const int MaximumWorkDurationMinutes = 180;
    public const int MinimumBreakDurationMinutes = 1;
    public const int MaximumBreakDurationMinutes = 60;

    public int WorkDurationMinutes { get; set; } = DefaultWorkDurationMinutes;

    public int BreakDurationMinutes { get; set; } = DefaultBreakDurationMinutes;

    public string LanguageCode { get; set; } = "zh-Hans";

    public bool AutoStartBreak { get; set; }

    public bool AutoStartNextWork { get; set; }

    public void Normalize()
    {
        WorkDurationMinutes = ClampWorkDuration(WorkDurationMinutes);
        BreakDurationMinutes = ClampBreakDuration(BreakDurationMinutes);

        if (string.IsNullOrWhiteSpace(LanguageCode))
        {
            LanguageCode = "zh-Hans";
        }
    }

    public static int ClampWorkDuration(int minutes)
    {
        return Math.Clamp(minutes, MinimumWorkDurationMinutes, MaximumWorkDurationMinutes);
    }

    public static int ClampBreakDuration(int minutes)
    {
        return Math.Clamp(minutes, MinimumBreakDurationMinutes, MaximumBreakDurationMinutes);
    }
}
