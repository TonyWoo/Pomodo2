# PomodoroTimer

[English README](README.md)

PomodoroTimer 是一个基于 Avalonia 构建的番茄钟应用。仓库现在将核心应用层共享给 Windows、macOS 桌面端，并提供 iOS 与 Android 平台入口，以复用同一套计时界面、状态逻辑与本地化能力。

## 项目概览

- 专注阶段：25 分钟
- 短休息阶段：5 分钟
- 支持开始/暂停、重置、切换到下一阶段
- 展示当前阶段进度和已完成的专注轮次
- 桌面端与移动端共享同一套 Avalonia 视图模型、状态模型和本地化逻辑

## 技术栈

- 共享应用层、桌面端与测试基于 .NET 8
- 由于当前 Avalonia Android/iOS 包的要求，移动端入口使用 .NET 10
- Avalonia 12
- CommunityToolkit.Mvvm
- xUnit

## 仓库结构

```text
.
|-- PomodoroTimer.sln
|-- PomodoroTimer.CrossPlatform.slnx
|-- PomodoroTimer/
|   |-- App.axaml
|   |-- Views/MainView.axaml
|   |-- Views/MainWindow.axaml
|   |-- ViewModels/MainWindowViewModel.cs
|   `-- Models/PomodoroTimerState.cs
|-- PomodoroTimer.Desktop/
|   `-- Program.cs
|-- PomodoroTimer.Android/
|   `-- MainActivity.cs
|-- PomodoroTimer.iOS/
|   `-- AppDelegate.cs
`-- PomodoroTimer.Tests/
    `-- PomodoroTimer.Tests.csproj
```

关键文件：

- `PomodoroTimer/App.axaml.cs`：共享 Avalonia 启动逻辑，根据桌面或移动生命周期装配同一套番茄钟体验
- `PomodoroTimer/Views/MainView.axaml`：桌面端与移动端共用的计时主界面
- `PomodoroTimer.Desktop/Program.cs`：Windows/macOS 桌面入口
- `PomodoroTimer.Android/MainActivity.cs`：Android 入口
- `PomodoroTimer.iOS/AppDelegate.cs`：iOS 入口

## 快速开始

### 前置条件

- 用于共享应用层、桌面端与测试的 .NET 8 SDK
- 构建移动端时还需要 .NET 10 SDK、Android/iOS workload 以及对应原生工具链

### 构建默认桌面与测试工作区

```bash
dotnet build PomodoroTimer.sln
```

### 在 Windows 或 macOS 运行桌面应用

```bash
dotnet run --project PomodoroTimer.Desktop/PomodoroTimer.Desktop.csproj
```

### 打开完整跨平台工作区

当你需要同时查看桌面、Android 与 iOS 项目时，可在 IDE 中打开 `PomodoroTimer.CrossPlatform.slnx`。

### 单独构建移动端入口

```bash
dotnet build PomodoroTimer.Android/PomodoroTimer.Android.csproj
dotnet build PomodoroTimer.iOS/PomodoroTimer.iOS.csproj
```

### 测试

```bash
dotnet test PomodoroTimer.Tests/PomodoroTimer.Tests.csproj --no-build
```

## 当前应用行为

- 应用启动后默认进入 25 分钟专注阶段
- 完成一轮专注后会切换到 5 分钟短休息
- 完成短休息后会回到专注阶段
- 手动切换阶段不会增加已完成专注轮次
- 默认界面文案为简体中文，应用内可切换其他语言

## 英文版本

GitHub 首页默认展示英文说明，详见 [README.md](README.md)。
