namespace PomodoroTimer.Localization;

public sealed record LanguageOption(AppLanguage Language, string Code, string DisplayName)
{
    public override string ToString() => DisplayName;
}
