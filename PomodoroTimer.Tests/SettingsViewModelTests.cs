using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using PomodoroTimer.ViewModels;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class SettingsViewModelTests
{
    [Fact]
    public void PresetsApplyExpectedDurationsAndSelectionStates()
    {
        var settings = new AppSettings();
        var viewModel = new SettingsViewModel(settings, new AppLocalizer(settings.LanguageCode), new InMemorySettingsStore(settings));

        viewModel.ApplyPresetCommand.Execute("50/10");

        Assert.Equal(50, viewModel.WorkDurationMinutes);
        Assert.Equal(10, viewModel.BreakDurationMinutes);
        Assert.False(viewModel.IsPreset25_5Active);
        Assert.True(viewModel.IsPreset50_10Active);
        Assert.False(viewModel.IsPresetCustomActive);

        viewModel.ApplyPresetCommand.Execute("25/5");

        Assert.Equal(25, viewModel.WorkDurationMinutes);
        Assert.Equal(5, viewModel.BreakDurationMinutes);
        Assert.True(viewModel.IsPreset25_5Active);
        Assert.False(viewModel.IsPreset50_10Active);
        Assert.False(viewModel.IsPresetCustomActive);
    }

    [Fact]
    public void DurationCommandsApplyCustomValuesAndSelectCustomMode()
    {
        var settings = new AppSettings();
        var viewModel = new SettingsViewModel(settings, new AppLocalizer(settings.LanguageCode), new InMemorySettingsStore(settings));

        viewModel.ApplyPresetCommand.Execute("50/10");

        viewModel.DecreaseWorkDurationCommand.Execute(null);
        viewModel.IncreaseBreakDurationCommand.Execute(null);

        Assert.Equal(49, settings.WorkDurationMinutes);
        Assert.Equal(11, settings.BreakDurationMinutes);
        Assert.False(viewModel.IsPreset25_5Active);
        Assert.False(viewModel.IsPreset50_10Active);
        Assert.True(viewModel.IsPresetCustomActive);
    }

    [Fact]
    public void SelectingCustomModeIsExclusiveEvenWhenDurationsMatchPreset()
    {
        var settings = new AppSettings();
        var viewModel = new SettingsViewModel(settings, new AppLocalizer(settings.LanguageCode), new InMemorySettingsStore(settings));

        viewModel.ApplyPresetCommand.Execute("50/10");
        viewModel.ApplyPresetCommand.Execute("custom");

        Assert.Equal(50, settings.WorkDurationMinutes);
        Assert.Equal(10, settings.BreakDurationMinutes);
        Assert.False(viewModel.IsPreset25_5Active);
        Assert.False(viewModel.IsPreset50_10Active);
        Assert.True(viewModel.IsPresetCustomActive);
    }

    [Fact]
    public void SelectingLanguageUpdatesSettingsAndLocalizerImmediately()
    {
        var settings = new AppSettings { LanguageCode = "en" };
        var localizer = new AppLocalizer(settings.LanguageCode);
        var viewModel = new SettingsViewModel(settings, localizer, new InMemorySettingsStore(settings));

        viewModel.SelectedLanguage = viewModel.LanguageOptions
            .Single(option => option.Language == AppLanguage.TraditionalChinese);

        Assert.Equal("zh-Hant", settings.LanguageCode);
        Assert.Equal(AppLanguage.TraditionalChinese, localizer.CurrentLanguage);
        Assert.Equal("設定", viewModel.Title);
    }

    private sealed class InMemorySettingsStore(AppSettings settings) : ISettingsStore
    {
        public AppSettings Settings { get; private set; } = settings;

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Settings);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            Settings = settings;
            return Task.CompletedTask;
        }
    }
}
