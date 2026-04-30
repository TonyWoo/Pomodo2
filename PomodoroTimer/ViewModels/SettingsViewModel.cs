using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;

namespace PomodoroTimer.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;
    private int _workDurationMinutes;
    private int _breakDurationMinutes;
    private LanguageOption _selectedLanguage;

    public SettingsViewModel(AppSettings settings, ISettingsStore settingsStore, AppLocalizer localizer)
    {
        _settings = AppSettingsNormalizer.Normalize(settings);
        _settingsStore = settingsStore;
        _localizer = localizer;
        _localizer.LanguageChanged += OnLanguageChanged;

        _workDurationMinutes = _settings.WorkDurationMinutes;
        _breakDurationMinutes = _settings.BreakDurationMinutes;
        _selectedLanguage = GetLanguageOption(_localizer.CurrentLanguage);

        ApplyPresetCommand = new RelayCommand<string?>(ApplyPreset);
        ChangeLanguageCommand = new RelayCommand<LanguageOption?>(ChangeLanguage);
        IncreaseWorkDurationCommand = new RelayCommand(() => WorkDurationMinutes++);
        DecreaseWorkDurationCommand = new RelayCommand(() => WorkDurationMinutes--);
        IncreaseBreakDurationCommand = new RelayCommand(() => BreakDurationMinutes++);
        DecreaseBreakDurationCommand = new RelayCommand(() => BreakDurationMinutes--);
    }

    public event EventHandler<AppSettings>? SettingsChanged;

    public IRelayCommand<string?> ApplyPresetCommand { get; }

    public IRelayCommand<LanguageOption?> ChangeLanguageCommand { get; }

    public IRelayCommand IncreaseWorkDurationCommand { get; }

    public IRelayCommand DecreaseWorkDurationCommand { get; }

    public IRelayCommand IncreaseBreakDurationCommand { get; }

    public IRelayCommand DecreaseBreakDurationCommand { get; }

    public IReadOnlyList<LanguageOption> LanguageOptions => _localizer.LanguageOptions;

    public int WorkDurationMinutes
    {
        get => _workDurationMinutes;
        set
        {
            var normalized = Math.Clamp(value, 1, 180);
            if (SetProperty(ref _workDurationMinutes, normalized))
            {
                _settings.WorkDurationMinutes = normalized;
                PersistSettings();
            }
        }
    }

    public int BreakDurationMinutes
    {
        get => _breakDurationMinutes;
        set
        {
            var normalized = Math.Clamp(value, 1, 60);
            if (SetProperty(ref _breakDurationMinutes, normalized))
            {
                _settings.BreakDurationMinutes = normalized;
                PersistSettings();
            }
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

    public string PageTitle => _localizer.GetText(LocalizedText.SettingsTitle);

    public string WorkDurationLabel => _localizer.GetText(LocalizedText.SettingsWorkDuration);

    public string BreakDurationLabel => _localizer.GetText(LocalizedText.SettingsBreakDuration);

    public string PresetsLabel => _localizer.GetText(LocalizedText.SettingsPresets);

    public string Preset255Label => _localizer.GetText(LocalizedText.SettingsPreset255);

    public string Preset505Label => _localizer.GetText(LocalizedText.SettingsPreset505);

    public string CustomLabel => _localizer.GetText(LocalizedText.SettingsCustom);

    public string LanguageLabel => _localizer.GetText(LocalizedText.SettingsLanguage);

    public string MinutesSuffix => _localizer.GetText(LocalizedText.SettingsMinutesSuffix);

    public string DecreaseLabel => _localizer.GetText(LocalizedText.ActionDecrease);

    public string IncreaseLabel => _localizer.GetText(LocalizedText.ActionIncrease);

    private void ApplyPreset(string? preset)
    {
        switch (preset)
        {
            case "25/5":
                WorkDurationMinutes = 25;
                BreakDurationMinutes = 5;
                break;
            case "50/5":
                WorkDurationMinutes = 50;
                BreakDurationMinutes = 5;
                break;
        }
    }

    private void ChangeLanguage(LanguageOption? languageOption)
    {
        if (languageOption is null)
        {
            return;
        }

        _settings.LanguageCode = languageOption.Code;
        _localizer.SetLanguage(languageOption.Language);
        PersistSettings();
    }

    private void PersistSettings()
    {
        var normalized = AppSettingsNormalizer.Normalize(_settings);
        SettingsChanged?.Invoke(this, normalized);
        _ = _settingsStore.SaveAsync(normalized);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        var language = GetLanguageOption(_localizer.CurrentLanguage);
        if (_selectedLanguage != language)
        {
            _selectedLanguage = language;
            OnPropertyChanged(nameof(SelectedLanguage));
        }

        RaiseLocalizedProperties();
    }

    private LanguageOption GetLanguageOption(AppLanguage language)
    {
        foreach (var option in LanguageOptions)
        {
            if (option.Language == language)
            {
                return option;
            }
        }

        return LanguageOptions[0];
    }

    private void RaiseLocalizedProperties()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(WorkDurationLabel));
        OnPropertyChanged(nameof(BreakDurationLabel));
        OnPropertyChanged(nameof(PresetsLabel));
        OnPropertyChanged(nameof(Preset255Label));
        OnPropertyChanged(nameof(Preset505Label));
        OnPropertyChanged(nameof(CustomLabel));
        OnPropertyChanged(nameof(LanguageLabel));
        OnPropertyChanged(nameof(MinutesSuffix));
        OnPropertyChanged(nameof(DecreaseLabel));
        OnPropertyChanged(nameof(IncreaseLabel));
    }
}
