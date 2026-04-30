using System;
using System.Collections.Generic;
using System.Globalization;

namespace PomodoroTimer.Localization;

public sealed class AppLocalizer
{
    private readonly ILanguagePreferenceStore? _preferenceStore;
    private readonly CultureInfo _systemCulture;

    private static readonly IReadOnlyDictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>> Catalog =
        new Dictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>>
        {
            [AppLanguage.SimplifiedChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.NavTimer] = "计时",
                [LocalizedText.NavStats] = "统计",
                [LocalizedText.NavSettings] = "设置",
                [LocalizedText.TimerWork] = "工作",
                [LocalizedText.TimerBreak] = "休息",
                [LocalizedText.TimerReady] = "准备开始",
                [LocalizedText.TimerRunning] = "专注中",
                [LocalizedText.TimerPaused] = "已暂停",
                [LocalizedText.TimerTopic] = "当前主题",
                [LocalizedText.TimerTopicPlaceholder] = "例如：撰写周报",
                [LocalizedText.TimerStart] = "开始",
                [LocalizedText.TimerPause] = "暂停",
                [LocalizedText.TimerResume] = "继续",
                [LocalizedText.TimerReset] = "重置",
                [LocalizedText.TimerStartBreak] = "开始休息",
                [LocalizedText.TimerStartNextWork] = "开始下一个番茄",
                [LocalizedText.TimerBreakHintFormat] = "休息 {0} 分钟",
                [LocalizedText.TimerTodayTasks] = "今日任务",
                [LocalizedText.TimerViewAll] = "查看全部",
                [LocalizedText.TimerNoSessions] = "今天还没有完成记录",
                [LocalizedText.StatsTodayPomodoros] = "今日番茄",
                [LocalizedText.StatsCompleted] = "已完成",
                [LocalizedText.StatsFocusMinutes] = "专注时长",
                [LocalizedText.StatsTodayFocusStats] = "今日专注统计",
                [LocalizedText.StatsCompletedFormat] = "已完成 {0} 个番茄",
                [LocalizedText.StatsFocusMinutesFormat] = "专注时长 {0} 分钟",
                [LocalizedText.StatsNoSessions] = "完成一个番茄后会显示今日记录",
                [LocalizedText.SettingsTitle] = "设置",
                [LocalizedText.SettingsWorkDuration] = "工作时长",
                [LocalizedText.SettingsBreakDuration] = "休息时长",
                [LocalizedText.SettingsPresets] = "预设方案",
                [LocalizedText.SettingsCustom] = "自定义",
                [LocalizedText.SettingsLanguage] = "语言",
                [LocalizedText.SettingsMinutes] = "分钟",
                [LocalizedText.SettingsPresetTwentyFiveFive] = "25 / 5",
                [LocalizedText.SettingsPresetFiftyFive] = "50 / 5",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "未命名任务",
                [LocalizedText.SessionTopic] = "主题",
                [LocalizedText.SessionCompletedAt] = "完成时间",
                [LocalizedText.SessionDuration] = "工作时长",
                [LocalizedText.SessionStatus] = "状态",
                [LocalizedText.SessionCompleted] = "已完成",
                [LocalizedText.SessionIncomplete] = "未完成",
            },
            [AppLanguage.TraditionalChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.NavTimer] = "計時",
                [LocalizedText.NavStats] = "統計",
                [LocalizedText.NavSettings] = "設定",
                [LocalizedText.TimerWork] = "工作",
                [LocalizedText.TimerBreak] = "休息",
                [LocalizedText.TimerReady] = "準備開始",
                [LocalizedText.TimerRunning] = "專注中",
                [LocalizedText.TimerPaused] = "已暫停",
                [LocalizedText.TimerTopic] = "目前主題",
                [LocalizedText.TimerTopicPlaceholder] = "例如：撰寫週報",
                [LocalizedText.TimerStart] = "開始",
                [LocalizedText.TimerPause] = "暫停",
                [LocalizedText.TimerResume] = "繼續",
                [LocalizedText.TimerReset] = "重設",
                [LocalizedText.TimerStartBreak] = "開始休息",
                [LocalizedText.TimerStartNextWork] = "開始下一個番茄",
                [LocalizedText.TimerBreakHintFormat] = "休息 {0} 分鐘",
                [LocalizedText.TimerTodayTasks] = "今日任務",
                [LocalizedText.TimerViewAll] = "查看全部",
                [LocalizedText.TimerNoSessions] = "今天還沒有完成記錄",
                [LocalizedText.StatsTodayPomodoros] = "今日番茄",
                [LocalizedText.StatsCompleted] = "已完成",
                [LocalizedText.StatsFocusMinutes] = "專注時長",
                [LocalizedText.StatsTodayFocusStats] = "今日專注統計",
                [LocalizedText.StatsCompletedFormat] = "已完成 {0} 個番茄",
                [LocalizedText.StatsFocusMinutesFormat] = "專注時長 {0} 分鐘",
                [LocalizedText.StatsNoSessions] = "完成一個番茄後會顯示今日記錄",
                [LocalizedText.SettingsTitle] = "設定",
                [LocalizedText.SettingsWorkDuration] = "工作時長",
                [LocalizedText.SettingsBreakDuration] = "休息時長",
                [LocalizedText.SettingsPresets] = "預設方案",
                [LocalizedText.SettingsCustom] = "自訂",
                [LocalizedText.SettingsLanguage] = "語言",
                [LocalizedText.SettingsMinutes] = "分鐘",
                [LocalizedText.SettingsPresetTwentyFiveFive] = "25 / 5",
                [LocalizedText.SettingsPresetFiftyFive] = "50 / 5",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "未命名任務",
                [LocalizedText.SessionTopic] = "主題",
                [LocalizedText.SessionCompletedAt] = "完成時間",
                [LocalizedText.SessionDuration] = "工作時長",
                [LocalizedText.SessionStatus] = "狀態",
                [LocalizedText.SessionCompleted] = "已完成",
                [LocalizedText.SessionIncomplete] = "未完成",
            },
            [AppLanguage.English] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodo Timer",
                [LocalizedText.NavTimer] = "Timer",
                [LocalizedText.NavStats] = "Stats",
                [LocalizedText.NavSettings] = "Settings",
                [LocalizedText.TimerWork] = "Work",
                [LocalizedText.TimerBreak] = "Break",
                [LocalizedText.TimerReady] = "Ready",
                [LocalizedText.TimerRunning] = "Focusing",
                [LocalizedText.TimerPaused] = "Paused",
                [LocalizedText.TimerTopic] = "Current Topic",
                [LocalizedText.TimerTopicPlaceholder] = "e.g. Write weekly report",
                [LocalizedText.TimerStart] = "Start",
                [LocalizedText.TimerPause] = "Pause",
                [LocalizedText.TimerResume] = "Resume",
                [LocalizedText.TimerReset] = "Reset",
                [LocalizedText.TimerStartBreak] = "Start Break",
                [LocalizedText.TimerStartNextWork] = "Start Next Pomodoro",
                [LocalizedText.TimerBreakHintFormat] = "Break {0} min",
                [LocalizedText.TimerTodayTasks] = "Today's Tasks",
                [LocalizedText.TimerViewAll] = "View All",
                [LocalizedText.TimerNoSessions] = "No completed sessions today",
                [LocalizedText.StatsTodayPomodoros] = "Today's Pomodoros",
                [LocalizedText.StatsCompleted] = "Completed",
                [LocalizedText.StatsFocusMinutes] = "Focus Time",
                [LocalizedText.StatsTodayFocusStats] = "Today's Focus Stats",
                [LocalizedText.StatsCompletedFormat] = "Completed {0} pomodoros",
                [LocalizedText.StatsFocusMinutesFormat] = "Focus time {0} minutes",
                [LocalizedText.StatsNoSessions] = "Complete a pomodoro to see today's records",
                [LocalizedText.SettingsTitle] = "Settings",
                [LocalizedText.SettingsWorkDuration] = "Work Duration",
                [LocalizedText.SettingsBreakDuration] = "Break Duration",
                [LocalizedText.SettingsPresets] = "Presets",
                [LocalizedText.SettingsCustom] = "Custom",
                [LocalizedText.SettingsLanguage] = "Language",
                [LocalizedText.SettingsMinutes] = "min",
                [LocalizedText.SettingsPresetTwentyFiveFive] = "25 / 5",
                [LocalizedText.SettingsPresetFiftyFive] = "50 / 5",
                [LocalizedText.LanguageSimplifiedChinese] = "简体中文",
                [LocalizedText.LanguageTraditionalChinese] = "繁體中文",
                [LocalizedText.LanguageEnglish] = "English",
                [LocalizedText.TaskUntitled] = "Untitled Task",
                [LocalizedText.SessionTopic] = "Topic",
                [LocalizedText.SessionCompletedAt] = "Completed",
                [LocalizedText.SessionDuration] = "Work Duration",
                [LocalizedText.SessionStatus] = "Status",
                [LocalizedText.SessionCompleted] = "Completed",
                [LocalizedText.SessionIncomplete] = "Incomplete",
            },
        };

    public AppLocalizer(string? languageCode = null)
        : this(preferenceStore: null, CultureInfo.CurrentUICulture, languageCode)
    {
    }

    public AppLocalizer(ILanguagePreferenceStore preferenceStore, CultureInfo? systemCulture = null)
        : this(preferenceStore, systemCulture ?? CultureInfo.CurrentUICulture, preferenceStore.LoadLanguageCode())
    {
    }

    private AppLocalizer(ILanguagePreferenceStore? preferenceStore, CultureInfo systemCulture, string? languageCode)
    {
        _preferenceStore = preferenceStore;
        _systemCulture = systemCulture;
        CurrentLanguage = ParseLanguageCode(languageCode) ?? ResolveSupportedLanguage(_systemCulture);
        LanguageOptions = BuildLanguageOptions();
    }

    public event EventHandler? LanguageChanged;

    public AppLanguage CurrentLanguage { get; private set; }

    public IReadOnlyList<LanguageOption> LanguageOptions { get; }

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

    public void SetLanguageCode(string languageCode)
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

        var normalized = languageCode.ToLowerInvariant();
        if (normalized.StartsWith("zh", StringComparison.Ordinal))
        {
            if (normalized.Contains("hant", StringComparison.Ordinal)
                || normalized.EndsWith("-tw", StringComparison.Ordinal)
                || normalized.EndsWith("-hk", StringComparison.Ordinal)
                || normalized.EndsWith("-mo", StringComparison.Ordinal))
            {
                return AppLanguage.TraditionalChinese;
            }

            return AppLanguage.SimplifiedChinese;
        }

        if (normalized.StartsWith("en", StringComparison.Ordinal))
        {
            return AppLanguage.English;
        }

        return null;
    }

    private static IReadOnlyList<LanguageOption> BuildLanguageOptions()
    {
        return
        [
            new(AppLanguage.SimplifiedChinese, Catalog[AppLanguage.SimplifiedChinese][LocalizedText.LanguageSimplifiedChinese]),
            new(AppLanguage.TraditionalChinese, Catalog[AppLanguage.SimplifiedChinese][LocalizedText.LanguageTraditionalChinese]),
            new(AppLanguage.English, Catalog[AppLanguage.SimplifiedChinese][LocalizedText.LanguageEnglish]),
        ];
    }
}
