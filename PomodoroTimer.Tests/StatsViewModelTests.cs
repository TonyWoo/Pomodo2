using System;
using System.Threading.Tasks;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using PomodoroTimer.ViewModels;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class StatsViewModelTests
{
    [Fact]
    public async Task RefreshCountsOnlyCompletedSessionsForToday()
    {
        var store = new InMemorySessionStore();
        var now = DateTimeOffset.Now;
        await store.SaveSessionAsync(CreateSession(now, completed: true));
        await store.SaveSessionAsync(CreateSession(now.AddMinutes(30), completed: false));
        await store.SaveSessionAsync(CreateSession(now.AddDays(-1), completed: true));
        var viewModel = new StatsViewModel(store, new AppLocalizer("en"));

        await viewModel.RefreshAsync();

        Assert.Equal(1, viewModel.CompletedPomodoros);
        Assert.Equal(25, viewModel.FocusMinutes);
        Assert.Equal(2, viewModel.Sessions.Count);
    }

    private static FocusSession CreateSession(DateTimeOffset endedAt, bool completed)
    {
        return new FocusSession
        {
            Topic = completed ? "Complete" : "Interrupted",
            StartedAt = endedAt.AddMinutes(-25),
            EndedAt = endedAt,
            PlannedMinutes = 25,
            ActualMinutes = completed ? 25 : 10,
            Completed = completed,
        };
    }
}
