using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class JsonTaskStore : ITaskStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string _tasksFilePath;

    public JsonTaskStore(IAppDataPathProvider pathProvider)
    {
        _tasksFilePath = Path.Combine(pathProvider.GetAppDataDirectory(), "tasks.json");
    }

    public async Task<IReadOnlyList<TodayTask>> LoadTasksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(_tasksFilePath))
            {
                return [];
            }

            await using var stream = File.OpenRead(_tasksFilePath);
            var tasks = await JsonSerializer.DeserializeAsync<List<TodayTask>>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            return tasks ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task SaveTaskAsync(TodayTask task, CancellationToken cancellationToken = default)
    {
        var tasks = (await LoadTasksAsync(cancellationToken).ConfigureAwait(false)).ToList();
        var existingIndex = tasks.FindIndex(existing => existing.Id == task.Id);
        if (existingIndex >= 0)
        {
            tasks[existingIndex] = task;
        }
        else
        {
            tasks.Add(task);
        }

        tasks = tasks
            .OrderBy(item => item.Completed)
            .ThenByDescending(item => item.CreatedAt)
            .ToList();

        var directory = Path.GetDirectoryName(_tasksFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_tasksFilePath);
        await JsonSerializer.SerializeAsync(stream, tasks, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var tasks = (await LoadTasksAsync(cancellationToken).ConfigureAwait(false))
            .Where(task => task.Id != taskId)
            .ToList();

        var directory = Path.GetDirectoryName(_tasksFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_tasksFilePath);
        await JsonSerializer.SerializeAsync(stream, tasks, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TodayTask>> GetTasksByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = DateOnly.FromDateTime(date);
        var tasks = await LoadTasksAsync(cancellationToken).ConfigureAwait(false);
        return tasks
            .Where(task => DateOnly.FromDateTime(task.CreatedAt.LocalDateTime) == targetDate)
            .OrderBy(task => task.Completed)
            .ThenByDescending(task => task.CreatedAt)
            .ToList();
    }
}
