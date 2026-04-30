using System;
using System.IO;
using System.Threading.Tasks;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class JsonSettingsStoreTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"pomodo-settings-{Guid.NewGuid():N}");
    private readonly JsonSettingsStore _store;

    public JsonSettingsStoreTests()
    {
        _store = new JsonSettingsStore(new TestPathProvider(_tempDirectory));
    }

    [Fact]
    public async Task MissingSettingsFileReturnsDefaults()
    {
        var settings = await _store.LoadAsync();

        Assert.Equal(25, settings.WorkDurationMinutes);
        Assert.Equal(5, settings.BreakDurationMinutes);
        Assert.Equal("zh-Hans", settings.LanguageCode);
    }

    [Fact]
    public async Task SavedDurationsAndLanguageReload()
    {
        await _store.SaveAsync(new AppSettings
        {
            WorkDurationMinutes = 50,
            BreakDurationMinutes = 5,
            LanguageCode = "zh-Hant",
        });

        var settings = await _store.LoadAsync();

        Assert.Equal(50, settings.WorkDurationMinutes);
        Assert.Equal(5, settings.BreakDurationMinutes);
        Assert.Equal("zh-Hant", settings.LanguageCode);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private sealed class TestPathProvider(string path) : IAppDataPathProvider
    {
        public string GetAppDataDirectory() => path;
    }
}
