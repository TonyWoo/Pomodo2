using System;
using System.IO;

namespace PomodoroTimer.Services;

public sealed class AppDataPathProvider : IAppDataPathProvider
{
    public string GetAppDataDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localAppData))
        {
            localAppData = AppContext.BaseDirectory;
        }

        return Path.Combine(localAppData, "PomodoTimer");
    }
}
