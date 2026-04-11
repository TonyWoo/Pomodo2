using System;
using System.Collections.Generic;
using System.Globalization;

namespace PomodoroTimer.Localization;

public sealed class AppLocalizer
{
    private static readonly IReadOnlyList<LanguageOption> SupportedLanguages =
    [
        new(AppLanguage.SimplifiedChinese, "简体中文"),
        new(AppLanguage.TraditionalChinese, "繁體中文"),
        new(AppLanguage.English, "English"),
    ];

    private static readonly IReadOnlyDictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>> Catalog =
        new Dictionary<AppLanguage, IReadOnlyDictionary<LocalizedText, string>>
        {
            [AppLanguage.SimplifiedChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "番茄钟",
                [LocalizedText.HeroTitle] = "用一段完整的时间，把今天最重要的事情往前推。",
                [LocalizedText.LanguageSelectionLabel] = "语言",
                [LocalizedText.PhaseFocusLabel] = "专注阶段",
                [LocalizedText.PhaseBreakLabel] = "短休息",
                [LocalizedText.PhaseFocusDescription] = "锁定一个任务，给它一整块不被打断的时间。",
                [LocalizedText.PhaseBreakDescription] = "离开屏幕几分钟，让下一轮专注重新变得清晰。",
                [LocalizedText.PrimaryActionStart] = "开始",
                [LocalizedText.PrimaryActionPause] = "暂停",
                [LocalizedText.NextPhaseToBreak] = "完成后进入 5 分钟短休息",
                [LocalizedText.NextPhaseToFocus] = "完成后回到 25 分钟专注",
                [LocalizedText.ProgressLabelFormat] = "{0:0}% 已完成",
                [LocalizedText.SessionLengthFocus] = "25 分钟专注块",
                [LocalizedText.SessionLengthBreak] = "5 分钟恢复块",
                [LocalizedText.CycleOutline] = "专注 25:00 -> 休息 05:00 -> 再次专注",
                [LocalizedText.FocusHintFocus] = "开始后每秒递减，结束时会自动切到休息阶段。",
                [LocalizedText.FocusHintBreak] = "这段时间用来站起来、补水，或者完全离开当前任务。",
                [LocalizedText.ResetActionLabel] = "重置",
                [LocalizedText.SkipPhaseActionLabel] = "切换阶段",
                [LocalizedText.CompletedRoundsLabel] = "完成轮次",
                [LocalizedText.CurrentPaceLabel] = "当前节奏",
                [LocalizedText.CycleStructureLabel] = "循环结构",
                [LocalizedText.HowToUseLabel] = "使用方式",
                [LocalizedText.HowToUseSteps] = "1. 点击开始进入当前阶段。\n2. 暂停会冻结倒计时。\n3. 重置会回到第一轮 25:00。",
                [LocalizedText.StatusReadyToStart] = "准备开始第一轮专注。",
                [LocalizedText.StatusFocusRunning] = "正在专注。把注意力留给这一件事。",
                [LocalizedText.StatusBreakRunning] = "正在休息。给自己一个真正的间隔。",
                [LocalizedText.StatusFocusPaused] = "计时已暂停，准备好时继续专注。",
                [LocalizedText.StatusBreakPaused] = "休息已暂停，准备好时继续。",
                [LocalizedText.StatusReset] = "计时器已重置，回到第一轮 25 分钟专注。",
                [LocalizedText.StatusSwitchedToBreak] = "已切换到短休息，准备重新整理节奏。",
                [LocalizedText.StatusFocusCompleted] = "一轮专注已完成，切换到短休息。",
                [LocalizedText.StatusSwitchedToFocus] = "已切回专注模式，下一轮可以直接开始。",
                [LocalizedText.StatusBreakCompleted] = "休息结束，下一轮 25 分钟专注已就绪。",
            },
            [AppLanguage.TraditionalChinese] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "番茄鐘",
                [LocalizedText.HeroTitle] = "用一段完整的時間，把今天最重要的事情往前推。",
                [LocalizedText.LanguageSelectionLabel] = "語言",
                [LocalizedText.PhaseFocusLabel] = "專注階段",
                [LocalizedText.PhaseBreakLabel] = "短休息",
                [LocalizedText.PhaseFocusDescription] = "鎖定一個任務，給它一整塊不被打斷的時間。",
                [LocalizedText.PhaseBreakDescription] = "離開螢幕幾分鐘，讓下一輪專注重新變得清晰。",
                [LocalizedText.PrimaryActionStart] = "開始",
                [LocalizedText.PrimaryActionPause] = "暫停",
                [LocalizedText.NextPhaseToBreak] = "完成後進入 5 分鐘短休息",
                [LocalizedText.NextPhaseToFocus] = "完成後回到 25 分鐘專注",
                [LocalizedText.ProgressLabelFormat] = "已完成 {0:0}%",
                [LocalizedText.SessionLengthFocus] = "25 分鐘專注塊",
                [LocalizedText.SessionLengthBreak] = "5 分鐘恢復塊",
                [LocalizedText.CycleOutline] = "專注 25:00 -> 休息 05:00 -> 再次專注",
                [LocalizedText.FocusHintFocus] = "開始後每秒遞減，結束時會自動切到休息階段。",
                [LocalizedText.FocusHintBreak] = "這段時間用來站起來、補水，或者完全離開當前任務。",
                [LocalizedText.ResetActionLabel] = "重置",
                [LocalizedText.SkipPhaseActionLabel] = "切換階段",
                [LocalizedText.CompletedRoundsLabel] = "完成輪次",
                [LocalizedText.CurrentPaceLabel] = "當前節奏",
                [LocalizedText.CycleStructureLabel] = "循環結構",
                [LocalizedText.HowToUseLabel] = "使用方式",
                [LocalizedText.HowToUseSteps] = "1. 點擊開始進入當前階段。\n2. 暫停會凍結倒計時。\n3. 重置會回到第一輪 25:00。",
                [LocalizedText.StatusReadyToStart] = "準備開始第一輪專注。",
                [LocalizedText.StatusFocusRunning] = "正在專注。把注意力留給這一件事。",
                [LocalizedText.StatusBreakRunning] = "正在休息。給自己一個真正的間隔。",
                [LocalizedText.StatusFocusPaused] = "計時已暫停，準備好時繼續專注。",
                [LocalizedText.StatusBreakPaused] = "休息已暫停，準備好時繼續。",
                [LocalizedText.StatusReset] = "計時器已重置，回到第一輪 25 分鐘專注。",
                [LocalizedText.StatusSwitchedToBreak] = "已切換到短休息，準備重新整理節奏。",
                [LocalizedText.StatusFocusCompleted] = "一輪專注已完成，切換到短休息。",
                [LocalizedText.StatusSwitchedToFocus] = "已切回專注模式，下一輪可以直接開始。",
                [LocalizedText.StatusBreakCompleted] = "休息結束，下一輪 25 分鐘專注已就緒。",
            },
            [AppLanguage.English] = new Dictionary<LocalizedText, string>
            {
                [LocalizedText.AppTitle] = "Pomodoro Timer",
                [LocalizedText.HeroTitle] = "Give your most important task one uninterrupted block of time.",
                [LocalizedText.LanguageSelectionLabel] = "Language",
                [LocalizedText.PhaseFocusLabel] = "Focus Session",
                [LocalizedText.PhaseBreakLabel] = "Short Break",
                [LocalizedText.PhaseFocusDescription] = "Pick one task and give it a full block of uninterrupted attention.",
                [LocalizedText.PhaseBreakDescription] = "Step away for a few minutes so the next focus round starts clear.",
                [LocalizedText.PrimaryActionStart] = "Start",
                [LocalizedText.PrimaryActionPause] = "Pause",
                [LocalizedText.NextPhaseToBreak] = "Then move into a 5-minute break",
                [LocalizedText.NextPhaseToFocus] = "Then return to a 25-minute focus block",
                [LocalizedText.ProgressLabelFormat] = "{0:0}% complete",
                [LocalizedText.SessionLengthFocus] = "25-minute focus block",
                [LocalizedText.SessionLengthBreak] = "5-minute recovery block",
                [LocalizedText.CycleOutline] = "Focus 25:00 -> Break 05:00 -> Focus again",
                [LocalizedText.FocusHintFocus] = "Once started, the timer counts down every second and switches to break automatically.",
                [LocalizedText.FocusHintBreak] = "Use this time to stand up, get water, or leave the task completely.",
                [LocalizedText.ResetActionLabel] = "Reset",
                [LocalizedText.SkipPhaseActionLabel] = "Skip Phase",
                [LocalizedText.CompletedRoundsLabel] = "Completed Rounds",
                [LocalizedText.CurrentPaceLabel] = "Current Pace",
                [LocalizedText.CycleStructureLabel] = "Cycle Structure",
                [LocalizedText.HowToUseLabel] = "How It Works",
                [LocalizedText.HowToUseSteps] = "1. Press Start for the current phase.\n2. Pause freezes the countdown.\n3. Reset goes back to the first 25:00 round.",
                [LocalizedText.StatusReadyToStart] = "Ready to begin the first focus round.",
                [LocalizedText.StatusFocusRunning] = "Focus mode is running. Keep your attention on one thing.",
                [LocalizedText.StatusBreakRunning] = "Break mode is running. Give yourself a real pause.",
                [LocalizedText.StatusFocusPaused] = "Focus is paused. Resume when you're ready.",
                [LocalizedText.StatusBreakPaused] = "Break is paused. Resume when you're ready.",
                [LocalizedText.StatusReset] = "Timer reset. Back to the first 25-minute focus round.",
                [LocalizedText.StatusSwitchedToBreak] = "Switched to a short break. Reset your pace before the next round.",
                [LocalizedText.StatusFocusCompleted] = "Focus round complete. Moving into a short break.",
                [LocalizedText.StatusSwitchedToFocus] = "Switched back to focus mode. The next round is ready to start.",
                [LocalizedText.StatusBreakCompleted] = "Break complete. The next 25-minute focus round is ready.",
            },
        };

    private readonly ILanguagePreferenceStore _preferenceStore;
    private readonly CultureInfo _systemCulture;

    public AppLocalizer(ILanguagePreferenceStore preferenceStore, CultureInfo? systemCulture = null)
    {
        _preferenceStore = preferenceStore;
        _systemCulture = systemCulture ?? CultureInfo.CurrentUICulture;
        CurrentLanguage = ResolveInitialLanguage();
    }

    public event EventHandler? LanguageChanged;

    public AppLanguage CurrentLanguage { get; private set; }

    public IReadOnlyList<LanguageOption> LanguageOptions => SupportedLanguages;

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
        _preferenceStore.SaveLanguageCode(ToCode(language));

        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public static AppLanguage ResolveSupportedLanguage(CultureInfo culture)
    {
        return ParseLanguageCode(culture.Name) ?? AppLanguage.English;
    }

    public static string ToCode(AppLanguage language)
    {
        return language switch
        {
            AppLanguage.SimplifiedChinese => "zh-CN",
            AppLanguage.TraditionalChinese => "zh-TW",
            _ => "en",
        };
    }

    private AppLanguage ResolveInitialLanguage()
    {
        return ParseLanguageCode(_preferenceStore.LoadLanguageCode())
            ?? ResolveSupportedLanguage(_systemCulture);
    }

    private static AppLanguage? ParseLanguageCode(string? languageCode)
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
}
