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
        Assert.Equal("准备开始第一轮专注。", state.StatusMessage);
    }

    [Fact]
    public void AdvanceWhileRunningDecrementsRemainingTime()
    {
        var state = CreateState();

        state.ToggleRunning();
        state.Advance(TimeSpan.FromSeconds(1));

        Assert.True(state.IsRunning);
        Assert.Equal(TimeSpan.FromSeconds(9), state.TimeRemaining);
        Assert.Equal("正在专注。把注意力留给这一件事。", state.StatusMessage);
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
        Assert.Equal("计时已暂停，准备好时继续专注。", state.StatusMessage);
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
        Assert.Equal("一轮专注已完成，切换到短休息。", state.StatusMessage);
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
        Assert.Equal("已切换到短休息，准备重新整理节奏。", state.StatusMessage);

        state.Reset();

        Assert.True(state.IsFocusSession);
        Assert.False(state.IsRunning);
        Assert.Equal(FocusDuration, state.TimeRemaining);
        Assert.Equal(0, state.CompletedFocusSessions);
        Assert.Equal("计时器已重置，回到第一轮 25 分钟专注。", state.StatusMessage);
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
        Assert.Equal("休息结束，下一轮 25 分钟专注已就绪。", state.StatusMessage);
    }

    private static PomodoroTimerState CreateState()
    {
        return new PomodoroTimerState(FocusDuration, BreakDuration);
    }
}
