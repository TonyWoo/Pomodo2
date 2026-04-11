# PomodoroTimer

[English README](README.md)

PomodoroTimer 是一个基于 Avalonia 构建的 .NET 8 桌面番茄钟应用。当前界面文案为中文，提供开始、暂停、重置和切换阶段等基础操作，用于完成一个简洁的专注/休息循环。

## 项目概览

- 专注阶段：25 分钟
- 短休息阶段：5 分钟
- 支持开始/暂停、重置、切换到下一阶段
- 展示当前阶段进度和已完成的专注轮次
- 使用 MVVM 组织界面逻辑，并为计时状态提供单元测试覆盖

## 技术栈

- .NET 8
- Avalonia 12
- CommunityToolkit.Mvvm
- xUnit

## 仓库结构

```text
.
|-- PomodoroTimer.sln
|-- PomodoroTimer/
|   |-- Program.cs
|   |-- App.axaml
|   |-- Views/MainWindow.axaml
|   |-- ViewModels/MainWindowViewModel.cs
|   `-- Models/PomodoroTimerState.cs
`-- PomodoroTimer.Tests/
    |-- PomodoroTimer.Tests.csproj
    `-- PomodoroTimerStateTests.cs
```

关键文件：

- `PomodoroTimer/Views/MainWindow.axaml`：主窗口布局、按钮和视觉样式
- `PomodoroTimer/ViewModels/MainWindowViewModel.cs`：界面绑定文本、命令和进度计算
- `PomodoroTimer/Models/PomodoroTimerState.cs`：专注/休息状态机与状态提示文案
- `PomodoroTimer.Tests/PomodoroTimerStateTests.cs`：计时状态相关回归测试

## 快速开始

### 前置条件

- .NET 8 SDK

### 构建

```bash
dotnet build PomodoroTimer.sln
```

### 运行

```bash
dotnet run --project PomodoroTimer/PomodoroTimer.csproj
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
- 当前界面文案为中文

## 英文版本

GitHub 首页默认展示英文说明，详见 [README.md](README.md)。

