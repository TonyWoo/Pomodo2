using System;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public static class AppSettingsNormalizer
{
    public static AppSettings Normalize(AppSettings? settings)
    {
        settings ??= new AppSettings();

        settings.WorkDurationMinutes = Math.Clamp(settings.WorkDurationMinutes, 1, 180);
        settings.BreakDurationMinutes = Math.Clamp(settings.BreakDurationMinutes, 1, 60);
        settings.LanguageCode = NormalizeLanguageCode(settings.LanguageCode);

        return settings;
    }

    public static string NormalizeLanguageCode(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return AppSettings.DefaultLanguageCode;
        }

        var normalized = languageCode.Trim();
        if (normalized.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-SG", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-Hans";
        }

        if (normalized.Equals("zh-Hant", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-TW", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-HK", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-MO", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-Hant";
        }

        if (normalized.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            return "en";
        }

        return AppSettings.DefaultLanguageCode;
    }
}
