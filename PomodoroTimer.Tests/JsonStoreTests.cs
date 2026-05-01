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

    [Fact]
    public async Task TaskStorePersistsUpdatesAndFiltersByLocalDate()
    {
        var store = new JsonTaskStore(_pathProvider);
        var today = DateTimeOffset.Now;
        var yesterday = today.AddDays(-1);
        var task = new TodayTask
        {
            Title = "Ship timer polish",
            CreatedAt = today,
        };

        await store.SaveTaskAsync(task);
        await store.SaveTaskAsync(new TodayTask
        {
            Title = "Older task",
            CreatedAt = yesterday,
        });

        task.Completed = true;
        task.CompletedPomodoros = 2;
        await store.SaveTaskAsync(task);

        var allTasks = await store.LoadTasksAsync();
        var todayTasks = await store.GetTasksByDateAsync(today.DateTime);

        Assert.Equal(2, allTasks.Count);
        Assert.Single(todayTasks);
        Assert.Equal("Ship timer polish", todayTasks.Single().Title);
        Assert.True(todayTasks.Single().Completed);
        Assert.Equal(2, todayTasks.Single().CompletedPomodoros);
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
