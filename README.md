# PomodoroTimer

一个基于 .NET 8 和 Avalonia 构建的番茄钟桌面应用，用来在 25 分钟专注和 5 分钟短休息之间切换。仓库同时包含桌面 UI、计时状态模型，以及覆盖核心节奏逻辑的 xUnit 测试，适合作为一个小型 Avalonia MVVM 示例项目来阅读和运行。

## 功能概览

- 25:00 专注阶段与 05:00 短休息阶段循环切换
- 支持开始、暂停、重置和手动切换阶段
- 展示倒计时、当前进度、下一阶段提示和已完成专注轮次
- 使用中文界面文案，单窗口即可完成完整交互

## 技术栈

- .NET 8
- Avalonia 12
- CommunityToolkit.Mvvm
- xUnit

## 项目结构

```text
.
|-- PomodoroTimer/
|   |-- Models/                    # 计时状态与阶段切换规则
|   |-- ViewModels/                # UI 绑定字段与命令
|   |-- Views/                     # Avalonia 主窗口布局
|   `-- Program.cs                 # 桌面应用入口
|-- PomodoroTimer.Tests/
|   `-- PomodoroTimerStateTests.cs # 核心计时逻辑测试
`-- PomodoroTimer.sln
```

## 本地运行

### 前置要求

- .NET SDK 8.0

### 构建项目

```bash
dotnet build PomodoroTimer.sln
```

### 运行桌面应用

```bash
dotnet run --project PomodoroTimer/PomodoroTimer.csproj
```

### 运行测试

```bash
dotnet test PomodoroTimer.sln
```

## 当前交互说明

- 应用默认从第一轮 25 分钟专注开始
- 专注阶段自然结束后会自动切换到 5 分钟短休息
- 手动切换阶段不会增加已完成专注轮次
- 重置会回到第一轮专注并清空已完成计数

## 代码定位

- `PomodoroTimer/Models/PomodoroTimerState.cs`：专注 / 休息切换、剩余时间与状态文案
- `PomodoroTimer/ViewModels/MainWindowViewModel.cs`：界面展示字段、按钮命令和进度计算
- `PomodoroTimer/Views/MainWindow.axaml`：主界面布局、配色和按钮区域
- `PomodoroTimer.Tests/PomodoroTimerStateTests.cs`：验证倒计时推进、暂停、重置与阶段切换行为
