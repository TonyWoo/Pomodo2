using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public interface ITaskStore
{
    Task<IReadOnlyList<TodayTask>> LoadTasksAsync(CancellationToken cancellationToken = default);

    Task SaveTaskAsync(TodayTask task, CancellationToken cancellationToken = default);

    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodayTask>> GetTasksByDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
