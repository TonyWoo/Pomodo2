using System.Reflection;
using PomodoroTimer.Localization;

namespace PomodoroTimer.ViewModels;

public sealed class AboutViewModel : ViewModelBase
{
    public const string Authors = "Tony Wu, Symphony, Codex";

    private readonly AppLocalizer _localizer;

    public AboutViewModel()
        : this(new AppLocalizer("zh-Hans"))
    {
    }

    public AboutViewModel(AppLocalizer localizer)
    {
        _localizer = localizer;
        Version = ResolveVersion();
    }

    public string Title => _localizer.GetText(LocalizedText.AboutTitle);

    public string VersionLabel => _localizer.GetText(LocalizedText.AboutVersionLabel);

    public string AuthorsLabel => _localizer.GetText(LocalizedText.AboutAuthorsLabel);

    public string Version { get; }

    public string AuthorsText => Authors;

    public void RefreshLocalization()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(VersionLabel));
        OnPropertyChanged(nameof(AuthorsLabel));
    }

    private static string ResolveVersion()
    {
        var assembly = typeof(AboutViewModel).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            return informationalVersion.Split('+')[0];
        }

        return assembly.GetName().Version?.ToString(3) ?? "Unknown";
    }
}
