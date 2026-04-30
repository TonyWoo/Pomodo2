using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public sealed class JsonSessionStore : ISessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _sessionsFilePath;

    public JsonSessionStore(IAppDataPathProvider pathProvider)
    {
        _sessionsFilePath = Path.Combine(pathProvider.GetAppDataDirectory(), "sessions.json");
    }

    public async Task<IReadOnlyList<FocusSession>> LoadSessionsAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            return await LoadSessionsCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveSessionAsync(FocusSession session)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            var sessions = (await LoadSessionsCoreAsync().ConfigureAwait(false)).ToList();
            var existingIndex = sessions.FindIndex(existing => existing.Id == session.Id);
            if (existingIndex >= 0)
            {
                sessions[existingIndex] = session;
            }
            else
            {
                sessions.Add(session);
            }

            var directory = Path.GetDirectoryName(_sessionsFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(sessions, JsonOptions);
            await File.WriteAllTextAsync(_sessionsFilePath, json).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date)
    {
        var sessions = await LoadSessionsAsync().ConfigureAwait(false);
        return sessions
            .Where(session => session.EndedAt.ToLocalTime().Date == date.Date)
            .OrderByDescending(session => session.EndedAt)
            .ToList();
    }

    private async Task<IReadOnlyList<FocusSession>> LoadSessionsCoreAsync()
    {
        try
        {
            if (!File.Exists(_sessionsFilePath))
            {
                return [];
            }

            var json = await File.ReadAllTextAsync(_sessionsFilePath).ConfigureAwait(false);
            return JsonSerializer.Deserialize<List<FocusSession>>(json, JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
