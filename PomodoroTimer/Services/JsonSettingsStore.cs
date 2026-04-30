using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly string _settingsFilePath;

    public JsonSettingsStore(IAppDataPathProvider pathProvider)
    {
        _settingsFilePath = Path.Combine(pathProvider.GetAppDataDirectory(), "settings.json");
    }

    public async Task<AppSettings> LoadAsync()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new AppSettings();
            }

            var json = await File.ReadAllTextAsync(_settingsFilePath).ConfigureAwait(false);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
            return AppSettingsNormalizer.Normalize(settings);
        }
        catch
        {
            return new AppSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        var normalized = AppSettingsNormalizer.Normalize(settings);
        var directory = Path.GetDirectoryName(_settingsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(normalized, JsonOptions);
        await File.WriteAllTextAsync(_settingsFilePath, json).ConfigureAwait(false);
    }
}
