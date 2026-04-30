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
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string _sessionsFilePath;

    public JsonSessionStore(IAppDataPathProvider pathProvider)
    {
        _sessionsFilePath = Path.Combine(pathProvider.GetAppDataDirectory(), "sessions.json");
    }

    public async Task<IReadOnlyList<FocusSession>> LoadSessionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(_sessionsFilePath))
            {
                return [];
            }

            await using var stream = File.OpenRead(_sessionsFilePath);
            var sessions = await JsonSerializer.DeserializeAsync<List<FocusSession>>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            return sessions ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task SaveSessionAsync(FocusSession session, CancellationToken cancellationToken = default)
    {
        var sessions = (await LoadSessionsAsync(cancellationToken).ConfigureAwait(false)).ToList();
        var existingIndex = sessions.FindIndex(existing => existing.Id == session.Id);
        if (existingIndex >= 0)
        {
            sessions[existingIndex] = session;
        }
        else
        {
            sessions.Add(session);
        }

        sessions = sessions.OrderBy(item => item.StartedAt).ToList();

        var directory = Path.GetDirectoryName(_sessionsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_sessionsFilePath);
        await JsonSerializer.SerializeAsync(stream, sessions, SerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<FocusSession>> GetSessionsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = DateOnly.FromDateTime(date);
        var sessions = await LoadSessionsAsync(cancellationToken).ConfigureAwait(false);
        return sessions
            .Where(session => DateOnly.FromDateTime(session.EndedAt.LocalDateTime) == targetDate)
            .OrderByDescending(session => session.EndedAt)
            .ToList();
    }
}
