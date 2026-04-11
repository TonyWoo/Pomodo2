using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Models;

namespace PomodoroTimer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly TimeSpan UiTickInterval = TimeSpan.FromSeconds(1);
    private readonly DispatcherTimer _timer;
    private readonly PomodoroTimerState _timerState;

    public MainWindowViewModel() : this(new PomodoroTimerState())
    {
    }

    internal MainWindowViewModel(PomodoroTimerState timerState)
    {
        _timerState = timerState;
        _timer = new DispatcherTimer { Interval = UiTickInterval };
        _timer.Tick += OnTimerTick;

        StartPauseCommand = new RelayCommand(ToggleTimer);
        ResetCommand = new RelayCommand(ResetTimer);
        SkipPhaseCommand = new RelayCommand(SkipPhase);
    }

    public IRelayCommand StartPauseCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public IRelayCommand SkipPhaseCommand { get; }

    public string PhaseLabel => _timerState.IsFocusSession ? "专注阶段" : "短休息";

    public string PhaseDescription => _timerState.IsFocusSession
        ? "锁定一个任务，给它一整块不被打断的时间。"
        : "离开屏幕几分钟，让下一轮专注重新变得清晰。";

    public string TimerDisplay => _timerState.TimeRemaining.ToString(@"mm\:ss");

    public string PrimaryActionLabel => _timerState.IsRunning ? "暂停" : "开始";

    public string NextPhaseLabel => _timerState.IsFocusSession
        ? "完成后进入 5 分钟短休息"
        : "完成后回到 25 分钟专注";

    public string ProgressLabel => $"{ProgressPercent:0}% 已完成";

    public double ProgressPercent
    {
        get
        {
            var totalSeconds = _timerState.CurrentDuration.TotalSeconds;
            if (totalSeconds <= 0)
            {
                return 0;
            }

            var elapsed = _timerState.CurrentDuration - _timerState.TimeRemaining;
            return Math.Clamp(elapsed.TotalSeconds / totalSeconds * 100, 0, 100);
        }
    }

    public string CompletedFocusSessions => _timerState.CompletedFocusSessions.ToString("D2");

    public string SessionLengthLabel => _timerState.IsFocusSession ? "25 分钟专注块" : "5 分钟恢复块";

    public string CycleOutline => "专注 25:00 → 休息 05:00 → 再次专注";

    public string FocusHint => _timerState.IsFocusSession
        ? "开始后每秒递减，结束时会自动切到休息阶段。"
        : "这段时间用来站起来、补水，或者完全离开当前任务。";

    public string StatusMessage => _timerState.StatusMessage;

    private void ToggleTimer()
    {
        _timerState.ToggleRunning();

        if (_timerState.IsRunning)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        RaiseUiStateChanged();
    }

    private void ResetTimer()
    {
        _timer.Stop();
        _timerState.Reset();
        RaiseUiStateChanged();
    }

    private void SkipPhase()
    {
        _timer.Stop();
        _timerState.SkipPhase();
        RaiseUiStateChanged();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _timerState.Advance(UiTickInterval);
        if (!_timerState.IsRunning)
        {
            _timer.Stop();
        }

        RaiseUiStateChanged();
    }

    private void RaiseUiStateChanged()
    {
        OnPropertyChanged(nameof(PhaseLabel));
        OnPropertyChanged(nameof(PhaseDescription));
        OnPropertyChanged(nameof(TimerDisplay));
        OnPropertyChanged(nameof(PrimaryActionLabel));
        OnPropertyChanged(nameof(NextPhaseLabel));
        OnPropertyChanged(nameof(ProgressLabel));
        OnPropertyChanged(nameof(ProgressPercent));
        OnPropertyChanged(nameof(CompletedFocusSessions));
        OnPropertyChanged(nameof(SessionLengthLabel));
        OnPropertyChanged(nameof(CycleOutline));
        OnPropertyChanged(nameof(FocusHint));
        OnPropertyChanged(nameof(StatusMessage));
    }
}
