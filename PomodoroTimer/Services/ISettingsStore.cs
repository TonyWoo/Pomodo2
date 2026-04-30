using System.Threading;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public interface ISettingsStore
{
    Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
