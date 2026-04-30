# Pomodo Timer

[English README](README.md)

Pomodo Timer 是一个基于 Avalonia 构建的番茄钟应用。仓库将核心应用层共享给 Windows、macOS 桌面端，并提供 iOS 与 Android 平台入口，以复用同一套计时界面、设置、专注记录、本地化和状态逻辑。

## 项目概览

- 默认工作时长：25 分钟
- 默认休息时长：5 分钟
- 支持 25 / 5、50 / 5 预设和自定义工作/休息时长
- 开始专注前可输入本次主题
- 支持开始、暂停、继续、重置
- 使用目标结束时间计算剩余时间，避免仅靠每秒递减导致的漂移
- 今日番茄数量和专注记录通过本地 JSON 持久化
- 支持简体中文、繁體中文和 English
- 桌面端与移动端共享同一套 Avalonia ViewModel、Model、Service 和本地化逻辑

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
|   |-- Services/
|   |-- Styles/
|   |-- Views/MainView.axaml
|   |-- Views/TimerView.axaml
|   |-- Views/StatsView.axaml
|   |-- Views/SettingsView.axaml
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
- `PomodoroTimer/Views/MainView.axaml`：桌面端与移动端共用的响应式外壳和导航
- `PomodoroTimer/Views/TimerView.axaml`：计时页，包含圆环进度、主题输入、操作按钮、今日番茄和记录
- `PomodoroTimer/Views/StatsView.axaml`：今日专注统计和记录列表
- `PomodoroTimer/Views/SettingsView.axaml`：时长预设、自定义时长和语言选择
- `PomodoroTimer/Services/`：计时、设置、专注记录和应用数据路径服务抽象
- `PomodoroTimer/Styles/`：Kingston 红、黑、白设计 Token 和共享控件样式
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

- 应用启动后默认进入计时页，工作 25 分钟、休息 5 分钟
- 用户可在开始工作前输入本次专注主题
- 完整完成工作倒计时后保存一条完成记录，并增加今日番茄数量
- 休息完成不会增加今日番茄数量
- 重置未完成的工作倒计时不会创建完成记录
- 设置和专注记录保存到本地 AppData JSON 文件
- 默认语言为简体中文，应用内可切换繁體中文和 English

## 英文版本

GitHub 首页默认展示英文说明，详见 [README.md](README.md)。
