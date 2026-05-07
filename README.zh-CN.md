# PomodoroTimer

[English README](README.md)

PomodoroTimer 是一个基于 Avalonia 构建的番茄钟应用。仓库现在将核心应用层共享给 Windows、macOS 桌面端，并提供 iOS 与 Android 平台入口，以复用同一套计时界面、状态逻辑与本地化能力。

## 项目概览

PomodoroTimer 是一款功能完整的生产力应用，将番茄工作法与任务管理和统计追踪相结合。应用在 Windows、macOS 桌面端、iOS 和 Android 平台上共享统一的代码库。

## 功能特性

- **番茄钟计时器**
  - 可自定义工作时长（默认 25 分钟）
  - 可自定义休息时长（默认 5 分钟）
  - 开始/暂停、重置和跳过控制
  - 圆形进度指示器的可视化计时显示
  
- **任务管理**
  - 创建和管理每日任务
  - 追踪每个任务完成的番茄钟数量
  - 标记任务为完成或删除任务
  - 任务在应用会话间持久保存
  
- **统计与历史**
  - 查看每日统计和专注时间
  - 浏览带时间戳的会话历史
  - 追踪一段时间内的生产力趋势
  
- **多语言支持**
  - 内置本地化系统
  - 应用内切换语言
  - 持久化语言偏好设置
  
- **多页面导航**
  - 计时器：主番茄钟界面及任务列表
  - 统计：每日统计和会话历史
  - 设置：自定义时长和语言
  - 关于：应用版本和信息
  
- **跨平台**
  - 桌面端和移动端的响应式布局
  - 所有平台共享 UI 和业务逻辑
  - 平台特定优化

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
|   |-- Views/
|   |   |-- MainView.axaml
|   |   |-- MainWindow.axaml
|   |   |-- TimerView.axaml
|   |   |-- StatsView.axaml
|   |   |-- SettingsView.axaml
|   |   `-- AboutView.axaml
|   |-- ViewModels/
|   |   |-- MainWindowViewModel.cs
|   |   |-- TimerViewModel.cs
|   |   |-- StatsViewModel.cs
|   |   |-- SettingsViewModel.cs
|   |   |-- AboutViewModel.cs
|   |   |-- TaskListItemViewModel.cs
|   |   `-- SessionListItemViewModel.cs
|   |-- Models/
|   |   |-- PomodoroTimerState.cs
|   |   |-- TodayTask.cs
|   |   |-- FocusSession.cs
|   |   |-- DailyStats.cs
|   |   `-- AppSettings.cs
|   |-- Services/
|   |   |-- TimerService.cs
|   |   |-- JsonTaskStore.cs
|   |   |-- JsonSessionStore.cs
|   |   |-- JsonSettingsStore.cs
|   |   `-- ITaskStore.cs / ISessionStore.cs / ISettingsStore.cs
|   `-- Localization/
|       |-- AppLocalizer.cs
|       |-- LocalizedText.cs
|       `-- LanguageOption.cs
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
- `PomodoroTimer/Views/MainView.axaml`：带多页面导航的共享计时器外壳
- `PomodoroTimer/ViewModels/TimerViewModel.cs`：计时器逻辑、任务协调和 UI 状态
- `PomodoroTimer/ViewModels/StatsViewModel.cs`：统计计算和会话历史
- `PomodoroTimer/Models/PomodoroTimerState.cs`：计时器状态机和阶段转换
- `PomodoroTimer/Models/TodayTask.cs`：带番茄钟计数的任务模型
- `PomodoroTimer/Services/TimerService.cs`：支持设置的核心计时器服务
- `PomodoroTimer/Services/JsonTaskStore.cs`：任务持久化层
- `PomodoroTimer/Localization/AppLocalizer.cs`：多语言支持和字符串管理
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

### 计时器与任务管理

- 应用启动后默认进入计时器页面，显示 25 分钟专注阶段
- 用户可以在番茄钟会话前或期间创建每日任务
- 在没有活动任务的情况下启动计时器会自动创建"无标题"任务
- 完成一轮专注后：
  - 为活动任务增加番茄钟计数
  - 将会话保存到历史记录
  - 切换到 5 分钟短休息
- 完成短休息后会回到专注阶段
- 手动跳过阶段不会增加已完成专注轮次
- 任务可以标记为完成或删除
- 所有任务和会话在应用重启后持久保存

### 导航与设置

- 使用侧边栏（桌面端）或底部导航（移动端）在计时器、统计、设置和关于页面之间导航
- 统计页面显示每日统计和会话历史
- 设置页面允许自定义工作/休息时长和语言选择
- 默认界面语言为简体中文，应用内可切换其他语言
- 所有设置在应用会话间持久保存

## 英文版本

GitHub 首页默认展示英文说明，详见 [README.md](README.md)。
