using System.Linq;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using PomodoroTimer.ViewModels;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public void SwitchingLanguageUpdatesVisibleCopy()
    {
        var settings = new AppSettings { LanguageCode = "en" };
        var localizer = new AppLocalizer(settings.LanguageCode);
        var viewModel = new MainWindowViewModel(
            settings,
            new InMemorySettingsStore(settings),
            new InMemorySessionStore(),
            localizer);

        Assert.Equal("Timer", viewModel.NavigationItems.Single(item => item.Page == AppPage.Timer).Label);
        Assert.Equal("Start", viewModel.Timer.StartButtonText);
        Assert.Equal("Current Topic", viewModel.Timer.TopicLabel);

        viewModel.Settings.SelectedLanguage = viewModel.Settings.LanguageOptions
            .Single(option => option.Language == AppLanguage.TraditionalChinese);

        Assert.Equal("計時", viewModel.NavigationItems.Single(item => item.Page == AppPage.Timer).Label);
        Assert.Equal("開始", viewModel.Timer.StartButtonText);
        Assert.Equal("目前主題", viewModel.Timer.TopicLabel);
        Assert.Equal("設定", viewModel.Settings.PageTitle);
    }

    [Fact]
    public void NavigationChangesCurrentPageViewModel()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Same(viewModel.Timer, viewModel.CurrentPageViewModel);

        viewModel.NavigateCommand.Execute(AppPage.Stats);

        Assert.Same(viewModel.Stats, viewModel.CurrentPageViewModel);
        Assert.True(viewModel.NavigationItems.Single(item => item.Page == AppPage.Stats).IsSelected);
    }
}
