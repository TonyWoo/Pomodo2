using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;

namespace PomodoroTimer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly TimeSpan UiTickInterval = TimeSpan.FromSeconds(1);
    private readonly AppLocalizer _localizer;
    private readonly DispatcherTimer _timer;
    private readonly PomodoroTimerState _timerState;
    private LanguageOption _selectedLanguage;

    public MainWindowViewModel()
        : this(new PomodoroTimerState(), new AppLocalizer(new InMemoryLanguagePreferenceStore()))
    {
    }

    public MainWindowViewModel(PomodoroTimerState timerState, AppLocalizer localizer)
    {
        _timerState = timerState;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;
        LanguageOptions = _localizer.LanguageOptions;
        _selectedLanguage = GetLanguageOption(_localizer.CurrentLanguage);
        _timer = new DispatcherTimer { Interval = UiTickInterval };
        _timer.Tick += OnTimerTick;

        StartPauseCommand = new RelayCommand(ToggleTimer);
        ResetCommand = new RelayCommand(ResetTimer);
        SkipPhaseCommand = new RelayCommand(SkipPhase);
    }

    public IRelayCommand StartPauseCommand { get; }

    public IRelayCommand ResetCommand { get; }

    public IRelayCommand SkipPhaseCommand { get; }

    public IReadOnlyList<LanguageOption> LanguageOptions { get; }

    public LanguageOption SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (value is null || value == _selectedLanguage)
            {
                return;
            }

            if (SetProperty(ref _selectedLanguage, value))
            {
                _localizer.SetLanguage(value.Language);
            }
        }
    }

    public string WindowTitle => _localizer.GetText(LocalizedText.AppTitle);

    public string HeroTitle => _localizer.GetText(LocalizedText.HeroTitle);

    public string LanguageSelectionLabel => _localizer.GetText(LocalizedText.LanguageSelectionLabel);

    public string PhaseLabel => _timerState.IsFocusSession
        ? _localizer.GetText(LocalizedText.PhaseFocusLabel)
        : _localizer.GetText(LocalizedText.PhaseBreakLabel);

    public string PhaseDescription => _timerState.IsFocusSession
        ? _localizer.GetText(LocalizedText.PhaseFocusDescription)
        : _localizer.GetText(LocalizedText.PhaseBreakDescription);

    public string TimerDisplay => _timerState.TimeRemaining.ToString(@"mm\:ss");

    public string PrimaryActionLabel => _timerState.IsRunning
        ? _localizer.GetText(LocalizedText.PrimaryActionPause)
        : _localizer.GetText(LocalizedText.PrimaryActionStart);

    public string NextPhaseLabel => _timerState.IsFocusSession
        ? _localizer.GetText(LocalizedText.NextPhaseToBreak)
        : _localizer.GetText(LocalizedText.NextPhaseToFocus);

    public string ProgressLabel => _localizer.Format(LocalizedText.ProgressLabelFormat, ProgressPercent);

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

    public string SessionLengthLabel => _timerState.IsFocusSession
        ? _localizer.GetText(LocalizedText.SessionLengthFocus)
        : _localizer.GetText(LocalizedText.SessionLengthBreak);

    public string CycleOutline => _localizer.GetText(LocalizedText.CycleOutline);

    public string FocusHint => _timerState.IsFocusSession
        ? _localizer.GetText(LocalizedText.FocusHintFocus)
        : _localizer.GetText(LocalizedText.FocusHintBreak);

    public string ResetActionLabel => _localizer.GetText(LocalizedText.ResetActionLabel);

    public string SkipPhaseActionLabel => _localizer.GetText(LocalizedText.SkipPhaseActionLabel);

    public string CompletedRoundsLabel => _localizer.GetText(LocalizedText.CompletedRoundsLabel);

    public string CurrentPaceLabel => _localizer.GetText(LocalizedText.CurrentPaceLabel);

    public string CycleStructureLabel => _localizer.GetText(LocalizedText.CycleStructureLabel);

    public string HowToUseLabel => _localizer.GetText(LocalizedText.HowToUseLabel);

    public string HowToUseSteps => _localizer.GetText(LocalizedText.HowToUseSteps);

    public string StatusMessage => _timerState.Status switch
    {
        PomodoroTimerStatus.ReadyToStart => _localizer.GetText(LocalizedText.StatusReadyToStart),
        PomodoroTimerStatus.FocusRunning => _localizer.GetText(LocalizedText.StatusFocusRunning),
        PomodoroTimerStatus.BreakRunning => _localizer.GetText(LocalizedText.StatusBreakRunning),
        PomodoroTimerStatus.FocusPaused => _localizer.GetText(LocalizedText.StatusFocusPaused),
        PomodoroTimerStatus.BreakPaused => _localizer.GetText(LocalizedText.StatusBreakPaused),
        PomodoroTimerStatus.Reset => _localizer.GetText(LocalizedText.StatusReset),
        PomodoroTimerStatus.SwitchedToBreak => _localizer.GetText(LocalizedText.StatusSwitchedToBreak),
        PomodoroTimerStatus.FocusCompleted => _localizer.GetText(LocalizedText.StatusFocusCompleted),
        PomodoroTimerStatus.SwitchedToFocus => _localizer.GetText(LocalizedText.StatusSwitchedToFocus),
        _ => _localizer.GetText(LocalizedText.StatusBreakCompleted),
    };

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

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        var nextLanguage = GetLanguageOption(_localizer.CurrentLanguage);
        if (_selectedLanguage != nextLanguage)
        {
            _selectedLanguage = nextLanguage;
            OnPropertyChanged(nameof(SelectedLanguage));
        }

        RaiseUiStateChanged();
    }

    private LanguageOption GetLanguageOption(AppLanguage language)
    {
        return LanguageOptions.First(option => option.Language == language);
    }

    private void RaiseUiStateChanged()
    {
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(HeroTitle));
        OnPropertyChanged(nameof(LanguageSelectionLabel));
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
        OnPropertyChanged(nameof(ResetActionLabel));
        OnPropertyChanged(nameof(SkipPhaseActionLabel));
        OnPropertyChanged(nameof(CompletedRoundsLabel));
        OnPropertyChanged(nameof(CurrentPaceLabel));
        OnPropertyChanged(nameof(CycleStructureLabel));
        OnPropertyChanged(nameof(HowToUseLabel));
        OnPropertyChanged(nameof(HowToUseSteps));
    }
}
