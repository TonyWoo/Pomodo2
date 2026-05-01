using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            localizer,
            new InMemorySettingsStore(settings),
            new InMemorySessionStore(),
            new InMemoryTaskStore(),
            [],
            []);

        Assert.Equal("Pomodo Timer", viewModel.WindowTitle);
        Assert.Equal("Timer", viewModel.NavTimerText);
        Assert.Equal("Start", viewModel.Timer.PrimaryActionText);

        viewModel.Settings.SelectedLanguage = viewModel.Settings.LanguageOptions
            .Single(option => option.Language == AppLanguage.TraditionalChinese);

        Assert.Equal("計時", viewModel.NavTimerText);
        Assert.Equal("開始", viewModel.Timer.PrimaryActionText);
        Assert.Equal("工作", viewModel.Timer.ModeText);
        Assert.Equal("準備開始", viewModel.Timer.StatusText);
    }

    [Fact]
    public void NavigationChangesCurrentPage()
    {
        var settings = new AppSettings();
        var viewModel = new MainWindowViewModel(
            settings,
            new AppLocalizer(settings.LanguageCode),
            new InMemorySettingsStore(settings),
            new InMemorySessionStore(),
            new InMemoryTaskStore(),
            [],
            []);

        Assert.True(viewModel.IsTimerPage);
        Assert.Same(viewModel.Timer, viewModel.CurrentPageViewModel);

        viewModel.NavigateCommand.Execute("Stats");

        Assert.True(viewModel.IsStatsPage);
        Assert.Same(viewModel.Stats, viewModel.CurrentPageViewModel);
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

    private sealed class InMemorySessionStore : ISessionStore
    {
        private readonly List<FocusSession> _sessions = [];

        public Task<IReadOnlyList<FocusSession>> LoadSessionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<FocusSession>>(_sessions);
        }

        public Task SaveSessionAsync(FocusSession session, CancellationToken cancellationToken = default)
        {
            _sessions.Add(session);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var targetDate = DateOnly.FromDateTime(date);
            IReadOnlyList<FocusSession> sessions = _sessions
                .Where(session => DateOnly.FromDateTime(session.EndedAt.LocalDateTime) == targetDate)
                .ToList();
            return Task.FromResult(sessions);
        }
    }

    private sealed class InMemoryTaskStore : ITaskStore
    {
        private readonly List<TodayTask> _tasks = [];

        public Task<IReadOnlyList<TodayTask>> LoadTasksAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TodayTask>>(_tasks);
        }

        public Task SaveTaskAsync(TodayTask task, CancellationToken cancellationToken = default)
        {
            var existingIndex = _tasks.FindIndex(existing => existing.Id == task.Id);
            if (existingIndex >= 0)
            {
                _tasks[existingIndex] = task;
            }
            else
            {
                _tasks.Add(task);
            }

            return Task.CompletedTask;
        }

        public Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            _tasks.RemoveAll(task => task.Id == taskId);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TodayTask>> GetTasksByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var targetDate = DateOnly.FromDateTime(date);
            IReadOnlyList<TodayTask> tasks = _tasks
                .Where(task => DateOnly.FromDateTime(task.CreatedAt.LocalDateTime) == targetDate)
                .ToList();
            return Task.FromResult(tasks);
        }
    }
}
