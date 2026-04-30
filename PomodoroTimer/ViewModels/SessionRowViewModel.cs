namespace PomodoroTimer.ViewModels;

public sealed class SessionRowViewModel
{
    public SessionRowViewModel(
        string topic,
        string completedTime,
        string workDuration,
        string statusText,
        bool completed)
    {
        Topic = topic;
        CompletedTime = completedTime;
        WorkDuration = workDuration;
        StatusText = statusText;
        Completed = completed;
    }

    public string Topic { get; }

    public string CompletedTime { get; }

    public string WorkDuration { get; }

    public string StatusText { get; }

    public bool Completed { get; }
}
