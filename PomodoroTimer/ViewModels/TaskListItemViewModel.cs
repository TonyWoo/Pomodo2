using System;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Localization;
using PomodoroTimer.Models;

namespace PomodoroTimer.ViewModels;

public sealed class TaskListItemViewModel : ViewModelBase
{
    private readonly AppLocalizer _localizer;
    private readonly Action<TodayTask> _useTask;
    private readonly Action<TodayTask> _toggleTask;
    private readonly Action<TodayTask> _deleteTask;
    private bool _isActive;

    public TaskListItemViewModel(
        TodayTask task,
        AppLocalizer localizer,
        Action<TodayTask> useTask,
        Action<TodayTask> toggleTask,
        Action<TodayTask> deleteTask)
    {
        Task = task;
        _localizer = localizer;
        _useTask = useTask;
        _toggleTask = toggleTask;
        _deleteTask = deleteTask;

        UseTaskCommand = new RelayCommand(() => _useTask(Task));
        ToggleCompletedCommand = new RelayCommand(() => _toggleTask(Task));
        DeleteTaskCommand = new RelayCommand(() => _deleteTask(Task));
    }

    public TodayTask Task { get; }

    public Guid Id => Task.Id;

    public string Title => Task.Title;

    public bool Completed => Task.Completed;

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public string PomodoroCountText => string.Format(
        _localizer.GetText(LocalizedText.TaskPomodoroCountFormat),
        Task.CompletedPomodoros);

    public string CompletionText => Task.Completed
        ? _localizer.GetText(LocalizedText.TaskCompleted)
        : _localizer.GetText(LocalizedText.TaskPending);

    public IRelayCommand UseTaskCommand { get; }

    public IRelayCommand ToggleCompletedCommand { get; }

    public IRelayCommand DeleteTaskCommand { get; }

    public void Refresh()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Completed));
        OnPropertyChanged(nameof(PomodoroCountText));
        OnPropertyChanged(nameof(CompletionText));
    }
}
