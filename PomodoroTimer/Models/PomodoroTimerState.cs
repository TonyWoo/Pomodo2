using System;

namespace PomodoroTimer.Models;

public sealed class PomodoroTimerState
{
    private readonly TimeSpan _focusDuration;
    private readonly TimeSpan _breakDuration;

    public PomodoroTimerState(TimeSpan? focusDuration = null, TimeSpan? breakDuration = null)
    {
        _focusDuration = focusDuration ?? TimeSpan.FromMinutes(25);
        _breakDuration = breakDuration ?? TimeSpan.FromMinutes(5);

        if (_focusDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(focusDuration), "Focus duration must be positive.");
        }

        if (_breakDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(breakDuration), "Break duration must be positive.");
        }

        IsFocusSession = true;
        TimeRemaining = _focusDuration;
        StatusMessage = "准备开始第一轮专注。";
    }

    public bool IsFocusSession { get; private set; }

    public bool IsRunning { get; private set; }

    public int CompletedFocusSessions { get; private set; }

    public TimeSpan TimeRemaining { get; private set; }

    public string StatusMessage { get; private set; }

    public TimeSpan CurrentDuration => IsFocusSession ? _focusDuration : _breakDuration;

    public void ToggleRunning()
    {
        IsRunning = !IsRunning;
        StatusMessage = IsRunning
            ? IsFocusSession
                ? "正在专注。把注意力留给这一件事。"
                : "正在休息。给自己一个真正的间隔。"
            : IsFocusSession
                ? "计时已暂停，准备好时继续专注。"
                : "休息已暂停，准备好时继续。";
    }

    public void Reset()
    {
        IsRunning = false;
        IsFocusSession = true;
        CompletedFocusSessions = 0;
        TimeRemaining = _focusDuration;
        StatusMessage = "计时器已重置，回到第一轮 25 分钟专注。";
    }

    public void SkipPhase()
    {
        CompleteCurrentPhase(manualAdvance: true);
    }

    public void Advance(TimeSpan elapsed)
    {
        if (!IsRunning || elapsed <= TimeSpan.Zero)
        {
            return;
        }

        if (elapsed < TimeRemaining)
        {
            TimeRemaining -= elapsed;
            return;
        }

        CompleteCurrentPhase(manualAdvance: false);
    }

    private void CompleteCurrentPhase(bool manualAdvance)
    {
        IsRunning = false;

        if (IsFocusSession)
        {
            if (!manualAdvance)
            {
                CompletedFocusSessions++;
            }

            IsFocusSession = false;
            TimeRemaining = _breakDuration;
            StatusMessage = manualAdvance
                ? "已切换到短休息，准备重新整理节奏。"
                : "一轮专注已完成，切换到短休息。";
            return;
        }

        IsFocusSession = true;
        TimeRemaining = _focusDuration;
        StatusMessage = manualAdvance
            ? "已切回专注模式，下一轮可以直接开始。"
            : "休息结束，下一轮 25 分钟专注已就绪。";
    }
}
