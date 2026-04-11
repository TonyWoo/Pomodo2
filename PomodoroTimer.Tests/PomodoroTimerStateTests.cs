using System;
using PomodoroTimer.Models;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class PomodoroTimerStateTests
{
    private static readonly TimeSpan FocusDuration = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(4);

    [Fact]
    public void StartsInFocusSessionWithInitialMessage()
    {
        var state = CreateState();

        Assert.True(state.IsFocusSession);
        Assert.False(state.IsRunning);
        Assert.Equal(FocusDuration, state.TimeRemaining);
        Assert.Equal(0, state.CompletedFocusSessions);
        Assert.Equal(PomodoroTimerStatus.ReadyToStart, state.Status);
    }

    [Fact]
    public void AdvanceWhileRunningDecrementsRemainingTime()
    {
        var state = CreateState();

        state.ToggleRunning();
        state.Advance(TimeSpan.FromSeconds(1));

        Assert.True(state.IsRunning);
        Assert.Equal(TimeSpan.FromSeconds(9), state.TimeRemaining);
        Assert.Equal(PomodoroTimerStatus.FocusRunning, state.Status);
    }

    [Fact]
    public void PausePreventsFurtherAdvances()
    {
        var state = CreateState();

        state.ToggleRunning();
        state.Advance(TimeSpan.FromSeconds(1));
        state.ToggleRunning();
        state.Advance(TimeSpan.FromSeconds(3));

        Assert.False(state.IsRunning);
        Assert.Equal(TimeSpan.FromSeconds(9), state.TimeRemaining);
        Assert.Equal(PomodoroTimerStatus.FocusPaused, state.Status);
    }

    [Fact]
    public void CompletingFocusSessionSwitchesToBreakAndCountsCompletion()
    {
        var state = CreateState();

        state.ToggleRunning();
        state.Advance(FocusDuration);

        Assert.False(state.IsRunning);
        Assert.False(state.IsFocusSession);
        Assert.Equal(BreakDuration, state.TimeRemaining);
        Assert.Equal(1, state.CompletedFocusSessions);
        Assert.Equal(PomodoroTimerStatus.FocusCompleted, state.Status);
    }

    [Fact]
    public void SkipAndResetKeepCountsHonest()
    {
        var state = CreateState();

        state.ToggleRunning();
        state.SkipPhase();

        Assert.False(state.IsFocusSession);
        Assert.Equal(BreakDuration, state.TimeRemaining);
        Assert.Equal(0, state.CompletedFocusSessions);
        Assert.Equal(PomodoroTimerStatus.SwitchedToBreak, state.Status);

        state.Reset();

        Assert.True(state.IsFocusSession);
        Assert.False(state.IsRunning);
        Assert.Equal(FocusDuration, state.TimeRemaining);
        Assert.Equal(0, state.CompletedFocusSessions);
        Assert.Equal(PomodoroTimerStatus.Reset, state.Status);
    }

    [Fact]
    public void CompletingBreakSessionReturnsToFocus()
    {
        var state = CreateState();

        state.SkipPhase();
        state.ToggleRunning();
        state.Advance(BreakDuration);

        Assert.False(state.IsRunning);
        Assert.True(state.IsFocusSession);
        Assert.Equal(FocusDuration, state.TimeRemaining);
        Assert.Equal(0, state.CompletedFocusSessions);
        Assert.Equal(PomodoroTimerStatus.BreakCompleted, state.Status);
    }

    private static PomodoroTimerState CreateState()
    {
        return new PomodoroTimerState(FocusDuration, BreakDuration);
    }
}
