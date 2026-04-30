using System;

namespace PomodoroTimer.Services;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
