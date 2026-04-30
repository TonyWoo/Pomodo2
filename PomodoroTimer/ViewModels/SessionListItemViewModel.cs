using System.Globalization;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;

namespace PomodoroTimer.ViewModels;

public sealed class SessionListItemViewModel : ViewModelBase
{
    private SessionListItemViewModel(
        string topic,
        string completedAtText,
        string durationText,
        string statusText,
        bool completed)
    {
        Topic = topic;
        CompletedAtText = completedAtText;
        DurationText = durationText;
        StatusText = statusText;
        Completed = completed;
    }

    public string Topic { get; }

    public string CompletedAtText { get; }

    public string DurationText { get; }

    public string StatusText { get; }

    public bool Completed { get; }

    public static SessionListItemViewModel FromSession(FocusSession session, AppLocalizer localizer)
    {
        var durationText = string.Create(
            CultureInfo.InvariantCulture,
            $"{session.PlannedMinutes} {localizer.GetText(LocalizedText.SettingsMinutes)}");

        return new SessionListItemViewModel(
            session.Topic,
            session.EndedAt.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture),
            durationText,
            session.Completed
                ? localizer.GetText(LocalizedText.SessionCompleted)
                : localizer.GetText(LocalizedText.SessionIncomplete),
            session.Completed);
    }
}
