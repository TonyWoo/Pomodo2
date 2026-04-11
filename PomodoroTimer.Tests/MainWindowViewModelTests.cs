using System;
using System.Globalization;
using System.Linq;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;
using PomodoroTimer.ViewModels;
using Xunit;

namespace PomodoroTimer.Tests;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public void SwitchingLanguageUpdatesVisibleCopy()
    {
        var timerState = new PomodoroTimerState(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(4));
        var localizer = new AppLocalizer(new InMemoryLanguagePreferenceStore(), new CultureInfo("en-US"));
        var viewModel = new MainWindowViewModel(timerState, localizer);

        Assert.Equal("Pomodoro Timer", viewModel.WindowTitle);
        Assert.Equal("Start", viewModel.PrimaryActionLabel);

        viewModel.SelectedLanguage = viewModel.LanguageOptions.Single(option => option.Language == AppLanguage.TraditionalChinese);

        Assert.Equal("番茄鐘", viewModel.WindowTitle);
        Assert.Equal("開始", viewModel.PrimaryActionLabel);
        Assert.Equal("專注階段", viewModel.PhaseLabel);
        Assert.Equal("準備開始第一輪專注。", viewModel.StatusMessage);
    }
}
