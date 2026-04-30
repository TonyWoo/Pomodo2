using CommunityToolkit.Mvvm.Input;

namespace PomodoroTimer.ViewModels;

public sealed class NavigationItemViewModel : ViewModelBase
{
    private string _label;
    private bool _isSelected;

    public NavigationItemViewModel(AppPage page, string label, IRelayCommand<AppPage> command)
    {
        Page = page;
        _label = label;
        Command = command;
    }

    public AppPage Page { get; }

    public IRelayCommand<AppPage> Command { get; }

    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
