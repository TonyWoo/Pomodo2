using System;
using System.Threading.Tasks;
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
    private readonly Func<string, string, Task<bool>>? _confirmDelete;
    private bool _isActive;

    public TaskListItemViewModel(
        TodayTask task,
        AppLocalizer localizer,
        Action<TodayTask> useTask,
        Action<TodayTask> toggleTask,
        Action<TodayTask> deleteTask,
        Func<string, string, Task<bool>>? confirmDelete = null)
    {
        Task = task;
        _localizer = localizer;
        _useTask = useTask;
        _toggleTask = toggleTask;
        _deleteTask = deleteTask;
        _confirmDelete = confirmDelete;

        UseTaskCommand = new RelayCommand(() => _useTask(Task));
        ToggleCompletedCommand = new RelayCommand(() => _toggleTask(Task));
        DeleteTaskCommand = new AsyncRelayCommand(ConfirmAndDeleteAsync);
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

    public IAsyncRelayCommand DeleteTaskCommand { get; }

    public string UseTaskTooltip => _localizer.GetText(LocalizedText.TaskUse);

    public string ToggleTooltip => Task.Completed
        ? _localizer.GetText(LocalizedText.TaskMarkPending)
        : _localizer.GetText(LocalizedText.TaskMarkDone);

    public string DeleteTaskTooltip => _localizer.GetText(LocalizedText.TaskDelete);

    public void Refresh()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Completed));
        OnPropertyChanged(nameof(PomodoroCountText));
        OnPropertyChanged(nameof(CompletionText));
        OnPropertyChanged(nameof(ToggleTooltip));
    }

    private async Task ConfirmAndDeleteAsync()
    {
        if (_confirmDelete is not null)
        {
            var confirmed = await _confirmDelete(
                _localizer.GetText(LocalizedText.TaskDeleteConfirmTitle),
                _localizer.GetText(LocalizedText.TaskDeleteConfirmMessage));
            if (!confirmed) return;
        }
        _deleteTask(Task);
    }
}
