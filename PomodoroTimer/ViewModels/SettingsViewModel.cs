using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private enum WorkModeSelection
    {
        TwentyFiveFive,
        FiftyTen,
        Custom,
    }

    private readonly AppLocalizer _localizer;
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;
    private LanguageOption _selectedLanguage;
    private WorkModeSelection _selectedWorkMode;

    public SettingsViewModel(AppSettings settings, AppLocalizer localizer, ISettingsStore settingsStore)
    {
        _settings = settings;
        _localizer = localizer;
        _settingsStore = settingsStore;
        LanguageOptions = _localizer.LanguageOptions;
        _selectedLanguage = LanguageOptions.First(option => option.Language == _localizer.CurrentLanguage);
        _selectedWorkMode = ResolveWorkModeSelection();

        IncreaseWorkDurationCommand = new RelayCommand(() => ChangeWorkDuration(1));
        DecreaseWorkDurationCommand = new RelayCommand(() => ChangeWorkDuration(-1));
        IncreaseBreakDurationCommand = new RelayCommand(() => ChangeBreakDuration(1));
        DecreaseBreakDurationCommand = new RelayCommand(() => ChangeBreakDuration(-1));
        ApplyPresetCommand = new RelayCommand<string>(ApplyPreset);
        ChangeLanguageCommand = new RelayCommand<LanguageOption>(ChangeLanguage);
    }

    public event EventHandler? SettingsChanged;

    public IRelayCommand IncreaseWorkDurationCommand { get; }

    public IRelayCommand DecreaseWorkDurationCommand { get; }

    public IRelayCommand IncreaseBreakDurationCommand { get; }

    public IRelayCommand DecreaseBreakDurationCommand { get; }

    public IRelayCommand<string> ApplyPresetCommand { get; }

    public IRelayCommand<LanguageOption> ChangeLanguageCommand { get; }

    public IReadOnlyList<LanguageOption> LanguageOptions { get; }

    public string Title => _localizer.GetText(LocalizedText.SettingsTitle);

    public string WorkDurationLabel => _localizer.GetText(LocalizedText.SettingsWorkDuration);

    public string BreakDurationLabel => _localizer.GetText(LocalizedText.SettingsBreakDuration);

    public string PresetsLabel => _localizer.GetText(LocalizedText.SettingsPresets);

    public string CustomLabel => _localizer.GetText(LocalizedText.SettingsCustom);

    public string LanguageLabel => _localizer.GetText(LocalizedText.SettingsLanguage);

    public string MinutesLabel => _localizer.GetText(LocalizedText.SettingsMinutes);

    public string PresetTwentyFiveFiveText => _localizer.GetText(LocalizedText.SettingsPresetTwentyFiveFive);

    public string PresetFiftyTenText => _localizer.GetText(LocalizedText.SettingsPresetFiftyTen);

    public bool IsPreset25_5Active => _selectedWorkMode == WorkModeSelection.TwentyFiveFive;
    public bool IsPreset50_10Active => _selectedWorkMode == WorkModeSelection.FiftyTen;
    public bool IsPresetCustomActive => _selectedWorkMode == WorkModeSelection.Custom;

    public int WorkDurationMinutes
    {
        get => _settings.WorkDurationMinutes;
        set
        {
            var next = AppSettings.ClampWorkDuration(value);
            if (_settings.WorkDurationMinutes == next)
            {
                return;
            }

            _settings.WorkDurationMinutes = next;
            _selectedWorkMode = WorkModeSelection.Custom;
            OnPropertyChanged();
            NotifyWorkModeSelectionChanged();
            PersistSettings();
        }
    }

    public int BreakDurationMinutes
    {
        get => _settings.BreakDurationMinutes;
        set
        {
            var next = AppSettings.ClampBreakDuration(value);
            if (_settings.BreakDurationMinutes == next)
            {
                return;
            }

            _settings.BreakDurationMinutes = next;
            _selectedWorkMode = WorkModeSelection.Custom;
            OnPropertyChanged();
            NotifyWorkModeSelectionChanged();
            PersistSettings();
        }
    }

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
                ChangeLanguage(value);
            }
        }
    }

    public void RefreshLocalization()
    {
        _selectedLanguage = LanguageOptions.First(option => option.Language == _localizer.CurrentLanguage);
        OnPropertyChanged(nameof(SelectedLanguage));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(WorkDurationLabel));
        OnPropertyChanged(nameof(BreakDurationLabel));
        OnPropertyChanged(nameof(PresetsLabel));
        OnPropertyChanged(nameof(CustomLabel));
        OnPropertyChanged(nameof(LanguageLabel));
        OnPropertyChanged(nameof(MinutesLabel));
        OnPropertyChanged(nameof(PresetTwentyFiveFiveText));
        OnPropertyChanged(nameof(PresetFiftyTenText));
    }

    private void ChangeWorkDuration(int delta)
    {
        WorkDurationMinutes += delta;
    }

    private void ChangeBreakDuration(int delta)
    {
        BreakDurationMinutes += delta;
    }

    private void ApplyPreset(string? preset)
    {
        switch (preset)
        {
            case "25/5":
                _settings.WorkDurationMinutes = 25;
                _settings.BreakDurationMinutes = 5;
                _selectedWorkMode = WorkModeSelection.TwentyFiveFive;
                break;
            case "50/10":
                _settings.WorkDurationMinutes = 50;
                _settings.BreakDurationMinutes = 10;
                _selectedWorkMode = WorkModeSelection.FiftyTen;
                break;
            case "custom":
                _selectedWorkMode = WorkModeSelection.Custom;
                break;
            default:
                return;
        }

        OnPropertyChanged(nameof(WorkDurationMinutes));
        OnPropertyChanged(nameof(BreakDurationMinutes));
        NotifyWorkModeSelectionChanged();
        PersistSettings();
    }

    private void ChangeLanguage(LanguageOption? option)
    {
        if (option is null)
        {
            return;
        }

        _settings.LanguageCode = AppLocalizer.ToCode(option.Language);
        _localizer.SetLanguage(option.Language);
        PersistSettings();
    }

    private async void PersistSettings()
    {
        _settings.Normalize();
        SettingsChanged?.Invoke(this, EventArgs.Empty);

        await _settingsStore.SaveAsync(_settings).ConfigureAwait(false);
    }

    private WorkModeSelection ResolveWorkModeSelection()
    {
        if (_settings.WorkDurationMinutes == 25 && _settings.BreakDurationMinutes == 5)
        {
            return WorkModeSelection.TwentyFiveFive;
        }

        if (_settings.WorkDurationMinutes == 50 && _settings.BreakDurationMinutes == 10)
        {
            return WorkModeSelection.FiftyTen;
        }

        return WorkModeSelection.Custom;
    }

    private void NotifyWorkModeSelectionChanged()
    {
        OnPropertyChanged(nameof(IsPreset25_5Active));
        OnPropertyChanged(nameof(IsPreset50_10Active));
        OnPropertyChanged(nameof(IsPresetCustomActive));
    }
}
