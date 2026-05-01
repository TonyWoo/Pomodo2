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

public sealed class TimerViewModelTests
{
    [Fact]
    public void AddTaskCommandCreatesTodayTaskImmediately()
    {
        var viewModel = CreateViewModel(out var taskStore);

        viewModel.Topic = "Write release notes";
        viewModel.AddTaskCommand.Execute(null);

        Assert.Single(viewModel.TodayTasks);
        Assert.Equal("Write release notes", viewModel.TodayTasks[0].Title);
        Assert.Single(taskStore.Tasks);
    }

    [Fact]
    public void StartCommandReusesExistingTaskInsteadOfDuplicatingIt()
    {
        var existingTask = new TodayTask
        {
            Title = "Refine sidebar",
            CreatedAt = DateTimeOffset.Now,
        };
        var viewModel = CreateViewModel(tasks: [existingTask]);

        viewModel.Topic = "Refine sidebar";
        viewModel.StartCommand.Execute(null);

        Assert.Single(viewModel.TodayTasks);
        Assert.Equal(existingTask.Id, viewModel.TodayTasks[0].Id);
    }

    private static TimerViewModel CreateViewModel(out InMemoryTaskStore taskStore, IEnumerable<TodayTask>? tasks = null)
    {
        taskStore = new InMemoryTaskStore(tasks);
        return new TimerViewModel(
            new TimerService(new AppSettings()),
            new AppLocalizer("en"),
            new InMemorySessionStore(),
            taskStore,
            [],
            tasks ?? []);
    }

    private static TimerViewModel CreateViewModel(IEnumerable<TodayTask>? tasks = null)
    {
        return CreateViewModel(out _, tasks);
    }

    private sealed class InMemorySessionStore : ISessionStore
    {
        public Task<IReadOnlyList<FocusSession>> LoadSessionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<FocusSession>>([]);
        }

        public Task SaveSessionAsync(FocusSession session, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<FocusSession>>([]);
        }
    }

    private sealed class InMemoryTaskStore(IEnumerable<TodayTask>? initialTasks = null) : ITaskStore
    {
        public List<TodayTask> Tasks { get; } = initialTasks?.ToList() ?? [];

        public Task<IReadOnlyList<TodayTask>> LoadTasksAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TodayTask>>(Tasks);
        }

        public Task SaveTaskAsync(TodayTask task, CancellationToken cancellationToken = default)
        {
            var existingIndex = Tasks.FindIndex(existing => existing.Id == task.Id);
            if (existingIndex >= 0)
            {
                Tasks[existingIndex] = task;
            }
            else
            {
                Tasks.Add(task);
            }

            return Task.CompletedTask;
        }

        public Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            Tasks.RemoveAll(task => task.Id == taskId);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TodayTask>> GetTasksByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var targetDate = DateOnly.FromDateTime(date);
            IReadOnlyList<TodayTask> tasks = Tasks
                .Where(task => DateOnly.FromDateTime(task.CreatedAt.LocalDateTime) == targetDate)
                .ToList();
            return Task.FromResult(tasks);
        }
    }
}
