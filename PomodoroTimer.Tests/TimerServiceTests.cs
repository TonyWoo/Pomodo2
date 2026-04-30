using System;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class TimerServiceTests
{
    [Fact]
    public void StartsInWorkModeWithDefaultDurations()
    {
        var service = new TimerService(new AppSettings());

        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(25), service.RemainingTime);
    }

    [Fact]
    public void StartPauseAndResumeUseClockBasedRemainingTime()
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 4, 30, 9, 0, 0, TimeSpan.Zero));
        var service = new TimerService(new AppSettings(), clock);

        service.Start("Review PR", "Untitled Task");
        clock.Advance(TimeSpan.FromSeconds(75));
        service.Tick();

        Assert.Equal(TimerStatus.Running, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(25) - TimeSpan.FromSeconds(75), service.RemainingTime);

        service.Pause();
        clock.Advance(TimeSpan.FromMinutes(10));
        service.Tick();

        Assert.Equal(TimerStatus.Paused, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(25) - TimeSpan.FromSeconds(75), service.RemainingTime);

        service.Start("Review PR", "Untitled Task");
        Assert.Equal(TimerStatus.Running, service.Status);
    }

    [Fact]
    public void CompletingWorkCreatesSessionAndDoesNotAutoStartBreakByDefault()
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 4, 30, 9, 0, 0, TimeSpan.Zero));
        var service = new TimerService(new AppSettings { WorkDurationMinutes = 1, BreakDurationMinutes = 5 }, clock);
        FocusSession? completedSession = null;
        service.WorkSessionCompleted += (_, session) => completedSession = session;

        service.Start("", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        service.Tick();

        Assert.NotNull(completedSession);
        Assert.Equal("Untitled Task", completedSession.Topic);
        Assert.True(completedSession.Completed);
        Assert.Equal(1, completedSession.PlannedMinutes);
        Assert.Equal(TimerMode.Break, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(5), service.RemainingTime);
    }

    [Fact]
    public void ResetAndBreakCompletionDoNotCreatePomodoroSessions()
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 4, 30, 9, 0, 0, TimeSpan.Zero));
        var service = new TimerService(new AppSettings { WorkDurationMinutes = 1, BreakDurationMinutes = 1 }, clock);
        var completedCount = 0;
        service.WorkSessionCompleted += (_, _) => completedCount++;

        service.Start("Mail", "Untitled Task");
        clock.Advance(TimeSpan.FromSeconds(30));
        service.Reset();

        Assert.Equal(0, completedCount);

        service.Start("Mail", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        service.Tick();
        service.Start("", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        service.Tick();

        Assert.Equal(1, completedCount);
        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
    }

    private sealed class FakeClock(DateTimeOffset now) : IClock
    {
        public DateTimeOffset Now { get; private set; } = now;

        public void Advance(TimeSpan elapsed)
        {
            Now += elapsed;
        }
    }
}
