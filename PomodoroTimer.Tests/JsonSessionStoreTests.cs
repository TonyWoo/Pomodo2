using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class JsonSessionStoreTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"pomodo-sessions-{Guid.NewGuid():N}");
    private readonly JsonSessionStore _store;

    public JsonSessionStoreTests()
    {
        _store = new JsonSessionStore(new TestPathProvider(_tempDirectory));
    }

    [Fact]
    public async Task SavedSessionCanBeLoaded()
    {
        var session = CreateSession(DateTimeOffset.Now);

        await _store.SaveSessionAsync(session);
        var sessions = await _store.LoadSessionsAsync();

        Assert.Single(sessions);
        Assert.Equal(session.Id, sessions[0].Id);
        Assert.Equal("Write report", sessions[0].Topic);
    }

    [Fact]
    public async Task DateFilterReturnsOnlyMatchingLocalDate()
    {
        var today = DateTime.Today;
        await _store.SaveSessionAsync(CreateSession(new DateTimeOffset(today.AddHours(9))));
        await _store.SaveSessionAsync(CreateSession(new DateTimeOffset(today.AddDays(-1).AddHours(9))));

        var sessions = await _store.GetSessionsByDateAsync(today);

        Assert.Single(sessions);
        Assert.Equal(today, sessions[0].EndedAt.ToLocalTime().Date);
    }

    [Fact]
    public async Task UpdatingSameSessionDoesNotDuplicateIt()
    {
        var session = CreateSession(DateTimeOffset.Now);
        await _store.SaveSessionAsync(session);

        session.Topic = "Updated";
        await _store.SaveSessionAsync(session);
        var sessions = await _store.LoadSessionsAsync();

        Assert.Single(sessions);
        Assert.Equal("Updated", sessions.Single().Topic);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private static FocusSession CreateSession(DateTimeOffset endedAt)
    {
        return new FocusSession
        {
            Topic = "Write report",
            StartedAt = endedAt.AddMinutes(-25),
            EndedAt = endedAt,
            PlannedMinutes = 25,
            ActualMinutes = 25,
            Completed = true,
        };
    }

    private sealed class TestPathProvider(string path) : IAppDataPathProvider
    {
        public string GetAppDataDirectory() => path;
    }
}
