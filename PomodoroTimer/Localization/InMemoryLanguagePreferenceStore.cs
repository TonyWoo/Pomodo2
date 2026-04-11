namespace PomodoroTimer.Localization;

public sealed class InMemoryLanguagePreferenceStore : ILanguagePreferenceStore
{
    private string? _languageCode;

    public InMemoryLanguagePreferenceStore(string? initialLanguageCode = null)
    {
        _languageCode = initialLanguageCode;
    }

    public string? LoadLanguageCode() => _languageCode;

    public void SaveLanguageCode(string languageCode)
    {
        _languageCode = languageCode;
    }
}
