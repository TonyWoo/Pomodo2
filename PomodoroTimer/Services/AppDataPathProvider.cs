using System;
using System.IO;

namespace PomodoroTimer.Services;

public sealed class AppDataPathProvider : IAppDataPathProvider
{
    private const string AppDataFolderName = "PomodoTimer";

    public string GetAppDataDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var root = string.IsNullOrWhiteSpace(localAppData)
            ? AppContext.BaseDirectory
            : localAppData;

        return Path.Combine(root, AppDataFolderName);
    }
}
