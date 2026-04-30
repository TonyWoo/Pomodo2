using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public interface ISessionStore
{
    Task<IReadOnlyList<FocusSession>> LoadSessionsAsync();

    Task SaveSessionAsync(FocusSession session);

    Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date);
}
