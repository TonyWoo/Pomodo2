using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace PomodoroTimer.Android;

[Activity(
    Label = "PomodoroTimer",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public sealed class MainActivity : AvaloniaMainActivity
{
}
