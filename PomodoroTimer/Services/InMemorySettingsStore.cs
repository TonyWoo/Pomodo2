using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class InMemorySettingsStore : ISettingsStore
{
    private AppSettings _settings;

    public InMemorySettingsStore(AppSettings? settings = null)
    {
        _settings = AppSettingsNormalizer.Normalize(settings);
    }

    public Task<AppSettings> LoadAsync()
    {
        return Task.FromResult(Clone(_settings));
    }

    public Task SaveAsync(AppSettings settings)
    {
        _settings = Clone(AppSettingsNormalizer.Normalize(settings));
        return Task.CompletedTask;
    }

    private static AppSettings Clone(AppSettings settings)
    {
        return new AppSettings
        {
            WorkDurationMinutes = settings.WorkDurationMinutes,
            BreakDurationMinutes = settings.BreakDurationMinutes,
            LanguageCode = settings.LanguageCode,
            AutoStartBreak = settings.AutoStartBreak,
            AutoStartNextWork = settings.AutoStartNextWork,
        };
    }
}
