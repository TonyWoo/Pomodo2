using System;
using System.Collections.Generic;
using System.Globalization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.Localization;

public sealed class AppLocalizer
{
    private static readonly IReadOnlyList<LanguageOption> SupportedLanguages =
    [
        new(AppLanguage.SimplifiedChinese, "zh-Hans", "简体中文"),
        new(AppLanguage.TraditionalChinese, "zh-Hant", "繁體中文"),
        new(AppLanguage.English, "en", "English"),
    ];

    private static readonly IReadOnlyDictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>> Catalog =
        new Dictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>>
        {
            [AppLanguage.SimplifiedChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.AppTagline] = "保持专注，记录今天的每个番茄。",
                [LocalizedText.NavTimer] = "计时",
                [LocalizedText.NavStats] = "统计",
                [LocalizedText.NavSettings] = "设置",
                [LocalizedText.TimerPageTitle] = "专注计时",
                [LocalizedText.TimerWork] = "工作",
                [LocalizedText.TimerBreak] = "休息",
                [LocalizedText.TimerReady] = "准备开始",
                [LocalizedText.TimerRunning] = "专注中",
                [LocalizedText.TimerBreakRunning] = "休息中",
                [LocalizedText.TimerPaused] = "已暂停",
                [LocalizedText.TimerWorkCompleted] = "工作完成",
                [LocalizedText.TimerBreakCompleted] = "休息完成",
                [LocalizedText.TimerTopic] = "当前主题",
                [LocalizedText.TimerTopicPlaceholder] = "例如：撰写周报",
                [LocalizedText.TimerStart] = "开始",
                [LocalizedText.TimerStartBreak] = "开始休息",
                [LocalizedText.TimerStartNext] = "开始下一个番茄",
                [LocalizedText.TimerPause] = "暂停",
                [LocalizedText.TimerResume] = "继续",
                [LocalizedText.TimerReset] = "重置",
                [LocalizedText.TimerBreakHintFormat] = "休息 {0} 分钟",
                [LocalizedText.TimerProgressFormat] = "{0:0}% 已完成",
                [LocalizedText.TimerTodayTasks] = "今日任务",
                [LocalizedText.TimerNoSessions] = "今天还没有完成记录。",
                [LocalizedText.TimerTaskTopicHeader] = "主题",
                [LocalizedText.TimerTaskCompletedTimeHeader] = "完成时间",
                [LocalizedText.TimerTaskDurationHeader] = "工作时长",
                [LocalizedText.TimerTaskStatusHeader] = "状态",
                [LocalizedText.StatsPageTitle] = "今日专注统计",
                [LocalizedText.StatsTodayPomodoros] = "今日番茄",
                [LocalizedText.StatsCompleted] = "已完成",
                [LocalizedText.StatsFocusMinutes] = "专注时长",
                [LocalizedText.StatsTodayFocusStats] = "今日专注统计",
                [LocalizedText.StatsSummaryFormat] = "已完成 {0} 个番茄",
                [LocalizedText.StatsSessionsTitle] = "完成记录",
                [LocalizedText.SettingsTitle] = "设置",
                [LocalizedText.SettingsWorkDuration] = "工作时长",
                [LocalizedText.SettingsBreakDuration] = "休息时长",
                [LocalizedText.SettingsPresets] = "预设方案",
                [LocalizedText.SettingsPreset255] = "25 / 5",
                [LocalizedText.SettingsPreset505] = "50 / 5",
                [LocalizedText.SettingsCustom] = "自定义",
                [LocalizedText.SettingsLanguage] = "语言",
                [LocalizedText.SettingsMinutesSuffix] = "分钟",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "未命名任务",
                [LocalizedText.StatusCompleted] = "已完成",
                [LocalizedText.StatusIncomplete] = "未完成",
                [LocalizedText.ActionDecrease] = "减少",
                [LocalizedText.ActionIncrease] = "增加",
            },
            [AppLanguage.TraditionalChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.AppTagline] = "保持專注，記錄今天的每個番茄。",
                [LocalizedText.NavTimer] = "計時",
                [LocalizedText.NavStats] = "統計",
                [LocalizedText.NavSettings] = "設定",
                [LocalizedText.TimerPageTitle] = "專注計時",
                [LocalizedText.TimerWork] = "工作",
                [LocalizedText.TimerBreak] = "休息",
                [LocalizedText.TimerReady] = "準備開始",
                [LocalizedText.TimerRunning] = "專注中",
                [LocalizedText.TimerBreakRunning] = "休息中",
                [LocalizedText.TimerPaused] = "已暫停",
                [LocalizedText.TimerWorkCompleted] = "工作完成",
                [LocalizedText.TimerBreakCompleted] = "休息完成",
                [LocalizedText.TimerTopic] = "目前主題",
                [LocalizedText.TimerTopicPlaceholder] = "例如：撰寫週報",
                [LocalizedText.TimerStart] = "開始",
                [LocalizedText.TimerStartBreak] = "開始休息",
                [LocalizedText.TimerStartNext] = "開始下一個番茄",
                [LocalizedText.TimerPause] = "暫停",
                [LocalizedText.TimerResume] = "繼續",
                [LocalizedText.TimerReset] = "重設",
                [LocalizedText.TimerBreakHintFormat] = "休息 {0} 分鐘",
                [LocalizedText.TimerProgressFormat] = "已完成 {0:0}%",
                [LocalizedText.TimerTodayTasks] = "今日任務",
                [LocalizedText.TimerNoSessions] = "今天還沒有完成記錄。",
                [LocalizedText.TimerTaskTopicHeader] = "主題",
                [LocalizedText.TimerTaskCompletedTimeHeader] = "完成時間",
                [LocalizedText.TimerTaskDurationHeader] = "工作時長",
                [LocalizedText.TimerTaskStatusHeader] = "狀態",
                [LocalizedText.StatsPageTitle] = "今日專注統計",
                [LocalizedText.StatsTodayPomodoros] = "今日番茄",
                [LocalizedText.StatsCompleted] = "已完成",
                [LocalizedText.StatsFocusMinutes] = "專注時長",
                [LocalizedText.StatsTodayFocusStats] = "今日專注統計",
                [LocalizedText.StatsSummaryFormat] = "已完成 {0} 個番茄",
                [LocalizedText.StatsSessionsTitle] = "完成記錄",
                [LocalizedText.SettingsTitle] = "設定",
                [LocalizedText.SettingsWorkDuration] = "工作時長",
                [LocalizedText.SettingsBreakDuration] = "休息時長",
                [LocalizedText.SettingsPresets] = "預設方案",
                [LocalizedText.SettingsPreset255] = "25 / 5",
                [LocalizedText.SettingsPreset505] = "50 / 5",
                [LocalizedText.SettingsCustom] = "自訂",
                [LocalizedText.SettingsLanguage] = "語言",
                [LocalizedText.SettingsMinutesSuffix] = "分鐘",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "未命名任務",
                [LocalizedText.StatusCompleted] = "已完成",
                [LocalizedText.StatusIncomplete] = "未完成",
                [LocalizedText.ActionDecrease] = "減少",
                [LocalizedText.ActionIncrease] = "增加",
            },
            [AppLanguage.English] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.AppTagline] = "Stay focused and count each pomodoro you finish today.",
                [LocalizedText.NavTimer] = "Timer",
                [LocalizedText.NavStats] = "Stats",
                [LocalizedText.NavSettings] = "Settings",
                [LocalizedText.TimerPageTitle] = "Focus Timer",
                [LocalizedText.TimerWork] = "Work",
                [LocalizedText.TimerBreak] = "Break",
                [LocalizedText.TimerReady] = "Ready",
                [LocalizedText.TimerRunning] = "Focusing",
                [LocalizedText.TimerBreakRunning] = "Resting",
                [LocalizedText.TimerPaused] = "Paused",
                [LocalizedText.TimerWorkCompleted] = "Work Complete",
                [LocalizedText.TimerBreakCompleted] = "Break Complete",
                [LocalizedText.TimerTopic] = "Current Topic",
                [LocalizedText.TimerTopicPlaceholder] = "e.g. Write weekly report",
                [LocalizedText.TimerStart] = "Start",
                [LocalizedText.TimerStartBreak] = "Start Break",
                [LocalizedText.TimerStartNext] = "Start Next Pomodoro",
                [LocalizedText.TimerPause] = "Pause",
                [LocalizedText.TimerResume] = "Resume",
                [LocalizedText.TimerReset] = "Reset",
                [LocalizedText.TimerBreakHintFormat] = "Break {0} minutes",
                [LocalizedText.TimerProgressFormat] = "{0:0}% complete",
                [LocalizedText.TimerTodayTasks] = "Today's Tasks",
                [LocalizedText.TimerNoSessions] = "No completed sessions today.",
                [LocalizedText.TimerTaskTopicHeader] = "Topic",
                [LocalizedText.TimerTaskCompletedTimeHeader] = "Completed",
                [LocalizedText.TimerTaskDurationHeader] = "Work Time",
                [LocalizedText.TimerTaskStatusHeader] = "Status",
                [LocalizedText.StatsPageTitle] = "Today's Focus Stats",
                [LocalizedText.StatsTodayPomodoros] = "Today's Pomodoros",
                [LocalizedText.StatsCompleted] = "Completed",
                [LocalizedText.StatsFocusMinutes] = "Focus Time",
                [LocalizedText.StatsTodayFocusStats] = "Today's Focus Stats",
                [LocalizedText.StatsSummaryFormat] = "{0} pomodoros completed",
                [LocalizedText.StatsSessionsTitle] = "Completed Records",
                [LocalizedText.SettingsTitle] = "Settings",
                [LocalizedText.SettingsWorkDuration] = "Work Duration",
                [LocalizedText.SettingsBreakDuration] = "Break Duration",
                [LocalizedText.SettingsPresets] = "Presets",
                [LocalizedText.SettingsPreset255] = "25 / 5",
                [LocalizedText.SettingsPreset505] = "50 / 5",
                [LocalizedText.SettingsCustom] = "Custom",
                [LocalizedText.SettingsLanguage] = "Language",
                [LocalizedText.SettingsMinutesSuffix] = "min",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "Untitled Task",
                [LocalizedText.StatusCompleted] = "Completed",
                [LocalizedText.StatusIncomplete] = "Incomplete",
                [LocalizedText.ActionDecrease] = "Decrease",
                [LocalizedText.ActionIncrease] = "Increase",
            },
        };

    private readonly ILanguagePreferenceStore? _preferenceStore;
    private readonly CultureInfo _systemCulture;

    public AppLocalizer(string? initialLanguageCode = null, CultureInfo? systemCulture = null)
    {
        _systemCulture = systemCulture ?? CultureInfo.CurrentUICulture;
        CurrentLanguage = ParseLanguageCode(initialLanguageCode)
            ?? ResolveSupportedLanguage(_systemCulture);
    }

    public AppLocalizer(ILanguagePreferenceStore preferenceStore, CultureInfo? systemCulture = null)
    {
        _preferenceStore = preferenceStore;
        _systemCulture = systemCulture ?? CultureInfo.CurrentUICulture;
        CurrentLanguage = ParseLanguageCode(_preferenceStore.LoadLanguageCode())
            ?? ResolveSupportedLanguage(_systemCulture);
    }

    public event EventHandler? LanguageChanged;

    public AppLanguage CurrentLanguage { get; private set; }

    public IReadOnlyList<LanguageOption> LanguageOptions => SupportedLanguages;

    public string CurrentLanguageCode => ToCode(CurrentLanguage);

    public string GetText(LocalizedText key)
    {
        if (Catalog[CurrentLanguage].TryGetValue(key, out var value))
        {
            return value;
        }

        return Catalog[AppLanguage.English][key];
    }

    public string Format(LocalizedText key, params object[] arguments)
    {
        return string.Format(CultureInfo.InvariantCulture, GetText(key), arguments);
    }

    public void SetLanguage(AppLanguage language)
    {
        _preferenceStore?.SaveLanguageCode(ToCode(language));

        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetLanguageCode(string? languageCode)
    {
        SetLanguage(ParseLanguageCode(languageCode) ?? AppLanguage.SimplifiedChinese);
    }

    public static AppLanguage ResolveSupportedLanguage(CultureInfo culture)
    {
        return ParseLanguageCode(culture.Name) ?? AppLanguage.English;
    }

    public static string ToCode(AppLanguage language)
    {
        return language switch
        {
            AppLanguage.SimplifiedChinese => "zh-Hans",
            AppLanguage.TraditionalChinese => "zh-Hant",
            _ => "en",
        };
    }

    public static AppLanguage? ParseLanguageCode(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return null;
        }

        var normalized = languageCode.Trim();
        if (normalized.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-SG", StringComparison.OrdinalIgnoreCase))
        {
            return AppLanguage.SimplifiedChinese;
        }

        if (normalized.Equals("zh-Hant", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-TW", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-HK", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("zh-MO", StringComparison.OrdinalIgnoreCase))
        {
            return AppLanguage.TraditionalChinese;
        }

        return normalized.StartsWith("en", StringComparison.OrdinalIgnoreCase)
            ? AppLanguage.English
            : null;
    }
}
