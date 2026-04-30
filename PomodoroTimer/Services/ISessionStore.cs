using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public interface ISessionStore
{
    Task<IReadOnlyList<FocusSession>> LoadSessionsAsync(CancellationToken cancellationToken = default);

    Task SaveSessionAsync(FocusSession session, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
