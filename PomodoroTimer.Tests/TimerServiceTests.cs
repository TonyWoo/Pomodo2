using System;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class TimerServiceTests
{
    [Fact]
    public void DefaultInitializationStartsAsIdleWorkTimer()
    {
        var service = CreateService();

        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(25), service.RemainingTime);
        Assert.Equal(0, service.Progress);
    }

    [Fact]
    public void StartPauseResumeAndResetFollowExpectedStateFlow()
    {
        var clock = new ManualClock();
        var service = CreateService(clock);

        service.Start("Write report", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(5));
        service.Tick();

        Assert.Equal(TimerStatus.Running, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(20), service.RemainingTime);

        service.Pause();
        clock.Advance(TimeSpan.FromMinutes(5));
        service.Tick();

        Assert.Equal(TimerStatus.Paused, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(20), service.RemainingTime);

        service.Resume();
        clock.Advance(TimeSpan.FromMinutes(5));
        service.Tick();

        Assert.Equal(TimerStatus.Running, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(15), service.RemainingTime);

        service.Reset();

        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(25), service.RemainingTime);
        Assert.Equal(0, service.Progress);
    }

    [Fact]
    public void CompletingWorkCreatesSessionAndMovesToBreakWithoutAutoStarting()
    {
        var clock = new ManualClock();
        var service = CreateService(clock, workMinutes: 1, breakMinutes: 2);

        service.Start("", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        var result = service.Tick();

        Assert.NotNull(result.CompletedSession);
        Assert.Equal(TimerCompletionKind.Work, result.CompletionKind);
        Assert.Equal("Untitled Task", result.CompletedSession.Topic);
        Assert.True(result.CompletedSession.Completed);
        Assert.Equal(1, result.CompletedSession.PlannedMinutes);
        Assert.Equal(TimerMode.Break, service.Mode);
        Assert.Equal(TimerStatus.Completed, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(2), service.RemainingTime);
    }

    [Fact]
    public void CompletingBreakDoesNotCreatePomodoroSession()
    {
        var clock = new ManualClock();
        var service = CreateService(clock, workMinutes: 1, breakMinutes: 1);

        service.Start("Read", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        service.Tick();

        service.Start("", "Untitled Task");
        clock.Advance(TimeSpan.FromMinutes(1));
        var result = service.Tick();

        Assert.Null(result.CompletedSession);
        Assert.Equal(TimerCompletionKind.Break, result.CompletionKind);
        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Completed, service.Status);
        Assert.Equal(TimeSpan.FromMinutes(1), service.RemainingTime);
    }

    [Fact]
    public void ResettingIncompleteWorkDoesNotCreateSession()
    {
        var clock = new ManualClock();
        var service = CreateService(clock, workMinutes: 1, breakMinutes: 1);

        service.Start("Draft", "Untitled Task");
        clock.Advance(TimeSpan.FromSeconds(20));
        service.Reset();
        var result = service.Tick();

        Assert.Null(result.CompletedSession);
        Assert.Equal(TimerCompletionKind.None, result.CompletionKind);
        Assert.Equal(TimerMode.Work, service.Mode);
        Assert.Equal(TimerStatus.Idle, service.Status);
    }

    private static TimerService CreateService(
        ManualClock? clock = null,
        int workMinutes = 25,
        int breakMinutes = 5)
    {
        return new TimerService(
            new AppSettings
            {
                WorkDurationMinutes = workMinutes,
                BreakDurationMinutes = breakMinutes,
            },
            clock ?? new ManualClock());
    }

    private sealed class ManualClock : IClock
    {
        public DateTimeOffset UtcNow { get; private set; } = new(2026, 4, 30, 9, 0, 0, TimeSpan.Zero);

        public void Advance(TimeSpan duration)
        {
            UtcNow = UtcNow.Add(duration);
        }
    }
}
