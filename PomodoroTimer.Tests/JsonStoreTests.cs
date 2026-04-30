using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class JsonStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(AppContext.BaseDirectory, "store-tests", Guid.NewGuid().ToString("N"));
    private readonly TestPathProvider _pathProvider;

    public JsonStoreTests()
    {
        _pathProvider = new TestPathProvider(_directory);
    }

    [Fact]
    public async Task SettingsStoreReturnsDefaultsWhenFileDoesNotExist()
    {
        var store = new JsonSettingsStore(_pathProvider);

        var settings = await store.LoadAsync();

        Assert.Equal(25, settings.WorkDurationMinutes);
        Assert.Equal(5, settings.BreakDurationMinutes);
        Assert.Equal("zh-Hans", settings.LanguageCode);
    }

    [Fact]
    public async Task SettingsStorePersistsDurationsAndLanguage()
    {
        var store = new JsonSettingsStore(_pathProvider);

        await store.SaveAsync(new AppSettings
        {
            WorkDurationMinutes = 50,
            BreakDurationMinutes = 5,
            LanguageCode = "zh-Hant",
        });

        var settings = await store.LoadAsync();

        Assert.Equal(50, settings.WorkDurationMinutes);
        Assert.Equal(5, settings.BreakDurationMinutes);
        Assert.Equal("zh-Hant", settings.LanguageCode);
    }

    [Fact]
    public async Task SessionStorePersistsAndFiltersByLocalDate()
    {
        var store = new JsonSessionStore(_pathProvider);
        var today = DateTimeOffset.Now;
        var yesterday = today.AddDays(-1);

        await store.SaveSessionAsync(new FocusSession
        {
            Topic = "Today",
            StartedAt = today.AddMinutes(-25),
            EndedAt = today,
            PlannedMinutes = 25,
            ActualMinutes = 25,
            Completed = true,
        });
        await store.SaveSessionAsync(new FocusSession
        {
            Topic = "Yesterday",
            StartedAt = yesterday.AddMinutes(-25),
            EndedAt = yesterday,
            PlannedMinutes = 25,
            ActualMinutes = 25,
            Completed = true,
        });

        var allSessions = await store.LoadSessionsAsync();
        var todaySessions = await store.GetSessionsByDateAsync(today.DateTime);

        Assert.Equal(2, allSessions.Count);
        Assert.Single(todaySessions);
        Assert.Equal("Today", todaySessions.Single().Topic);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private sealed class TestPathProvider(string directory) : IAppDataPathProvider
    {
        public string GetAppDataDirectory() => directory;
    }
}
