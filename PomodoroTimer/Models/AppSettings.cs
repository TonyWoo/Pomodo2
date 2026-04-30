namespace PomodoroTimer.Models;

public sealed class AppSettings
{
    public const int DefaultWorkDurationMinutes = 25;
    public const int DefaultBreakDurationMinutes = 5;
    public const string DefaultLanguageCode = "zh-Hans";

    public int WorkDurationMinutes { get; set; } = DefaultWorkDurationMinutes;

    public int BreakDurationMinutes { get; set; } = DefaultBreakDurationMinutes;

    public string LanguageCode { get; set; } = DefaultLanguageCode;

    public bool AutoStartBreak { get; set; }

    public bool AutoStartNextWork { get; set; }
}
