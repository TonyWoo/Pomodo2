using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class InMemorySessionStore : ISessionStore
{
    private readonly List<FocusSession> _sessions = [];

    public Task<IReadOnlyList<FocusSession>> LoadSessionsAsync()
    {
        return Task.FromResult<IReadOnlyList<FocusSession>>(_sessions.ToList());
    }

    public Task SaveSessionAsync(FocusSession session)
    {
        var index = _sessions.FindIndex(existing => existing.Id == session.Id);
        if (index >= 0)
        {
            _sessions[index] = session;
        }
        else
        {
            _sessions.Add(session);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date)
    {
        var sessions = _sessions
            .Where(session => session.EndedAt.ToLocalTime().Date == date.Date)
            .OrderByDescending(session => session.EndedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<FocusSession>>(sessions);
    }
}
