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
    public void PresetsAndDurationCommandsUpdateSettings()
    {
        var settings = new AppSettings();
        var viewModel = new SettingsViewModel(settings, new AppLocalizer(settings.LanguageCode), new InMemorySettingsStore(settings));

        viewModel.ApplyPresetCommand.Execute("50/5");

        Assert.Equal(50, viewModel.WorkDurationMinutes);
        Assert.Equal(5, viewModel.BreakDurationMinutes);

        viewModel.DecreaseWorkDurationCommand.Execute(null);
        viewModel.IncreaseBreakDurationCommand.Execute(null);

        Assert.Equal(49, settings.WorkDurationMinutes);
        Assert.Equal(6, settings.BreakDurationMinutes);
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
