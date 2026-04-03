# 开发文档

## 项目概述

这是一个运行在 Windows 上的锁屏程序，当前技术栈为：
- .NET 8
- WPF
- Win32 全局快捷键注册

当前目标是先保持代码结构简单，在 MVP 基础上逐步扩展为可配置、可个性化的锁屏应用。

## 当前行为

1. 程序启动后在后台保持运行。
2. `MainWindow` 作为隐藏控制窗口存在。
3. 控制窗口通过 `RegisterHotKey` 注册 `Ctrl+Alt+L`。
4. 用户按下快捷键后，打开全屏 `LockScreenWindow`。
5. 用户输入本地 PIN 后解锁。

## 关键文件

- `src/LockScreen.App/App.xaml.cs`
  负责应用启动和显式关闭模式。
- `src/LockScreen.App/MainWindow.xaml.cs`
  负责隐藏控制窗口和全局快捷键注册。
- `src/LockScreen.App/LockScreenWindow.xaml`
  负责 MVP 锁屏界面布局。
- `src/LockScreen.App/LockScreenWindow.xaml.cs`
  负责 PIN 校验和窗口关闭拦截。

## 当前设计取舍

- 保留隐藏的 `MainWindow`，是为了给 `RegisterHotKey` 提供稳定的窗口句柄。
- 锁屏界面独立为 `LockScreenWindow`，方便后续继续扩展 UI 和个性化模块。
- MVP 阶段将 PIN 写死，目的是先确保主流程可运行、可验证。

## 当前限制

- 这是应用层锁屏，不是 Windows 系统级锁屏替代品。
- PIN 目前没有做安全存储。
- 还没有托盘图标、设置页面和配置持久化。
- 还没有覆盖多显示器场景。

## 建议的下一步

### 1. 托盘支持

增加托盘图标，用于：
- 退出程序
- 显示运行状态
- 打开设置页面

### 2. PIN 配置化

把当前写死的 PIN 移到本地配置文件或更安全的存储方案中。

建议新增：
- `Models/AppConfig.cs`
- `Services/ConfigService.cs`
- `Services/AuthService.cs`

### 3. 个性化内容

逐步加入锁屏内容模块：
- 时钟
- 壁纸轮播
- 每日一句
- 待办摘要
- 天气

### 4. 空闲触发

通过 `GetLastInputInfo` 检测用户空闲时间，在达到阈值后自动进入锁屏界面。

## 构建

```powershell
dotnet build .\LockScreen.sln
```

## 运行

```powershell
dotnet run --project .\src\LockScreen.App
```
