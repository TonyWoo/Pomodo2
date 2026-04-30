using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services;

public interface ISettingsStore
{
    Task<AppSettings> LoadAsync();

    Task SaveAsync(AppSettings settings);
}
