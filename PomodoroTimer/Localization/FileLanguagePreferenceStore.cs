using System;
using System.IO;
using System.Text.Json;

namespace PomodoroTimer.Localization;

public sealed class FileLanguagePreferenceStore : ILanguagePreferenceStore
{
    private readonly string _settingsFilePath;

    public FileLanguagePreferenceStore(string? settingsFilePath = null)
    {
        _settingsFilePath = settingsFilePath
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PomodoroTimer",
                "settings.json");
    }

    public string? LoadLanguageCode()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(_settingsFilePath);
            return JsonSerializer.Deserialize<LanguageSettingsDocument>(json)?.Language;
        }
        catch
        {
            return null;
        }
    }

    public void SaveLanguageCode(string languageCode)
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(
                new LanguageSettingsDocument { Language = languageCode },
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_settingsFilePath, json);
        }
        catch
        {
        }
    }

    private sealed class LanguageSettingsDocument
    {
        public string? Language { get; set; }
    }
}
