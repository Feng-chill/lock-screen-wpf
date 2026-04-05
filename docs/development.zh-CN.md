# 开发文档

## 项目概述

这是一个运行在 Windows 上的锁屏程序，当前技术栈为：
- .NET 8
- WPF
- Win32 全局快捷键注册

当前目标是先保持代码结构简单，在 MVP 基础上逐步扩展为可配置、可个性化的锁屏应用。

当前版本不是 Windows 系统级锁屏替代品，而是一个应用层的自定义锁屏程序。它的核心价值是：
- 程序常驻后台
- 监听全局快捷键
- 快速弹出全屏锁屏界面
- 用最短路径验证交互闭环

## 当前 MVP 能做什么

当前 MVP 已实现以下能力：
- 程序启动后在后台保持运行
- 注册全局快捷键 `Ctrl+Alt+L`
- 用户按下快捷键后打开全屏锁屏窗口
- 锁屏窗口默认置顶、全屏、无边框
- 用户输入本地 PIN 后解锁
- 对普通关闭行为做基础拦截，避免误退出锁屏页

默认 PIN 为：`1234`

## 项目结构

当前项目结构比较精简，只有一个 WPF 应用项目：

```text
lock-screen_v1/
├─ LockScreen.sln
├─ README.md
├─ README.zh-CN.md
├─ docs/
│  ├─ development.md
│  └─ development.zh-CN.md
└─ src/
   └─ LockScreen.App/
      ├─ App.xaml
      ├─ App.xaml.cs
      ├─ AssemblyInfo.cs
      ├─ LockScreen.App.csproj
      ├─ MainWindow.xaml
      ├─ MainWindow.xaml.cs
      ├─ LockScreenWindow.xaml
      └─ LockScreenWindow.xaml.cs
```

## 各文件职责

### 1. 解决方案与说明文件

- `LockScreen.sln`
  解决方案入口，便于 Visual Studio 或 `dotnet` 命令统一管理项目。
- `README.md` / `README.zh-CN.md`
  面向仓库访问者的快速说明。
- `docs/development.md` / `docs/development.zh-CN.md`
  面向开发者的实现说明与后续规划。

### 2. 应用入口层

- `src/LockScreen.App/App.xaml`
  WPF 应用资源入口，目前没有定义额外资源。
- `src/LockScreen.App/App.xaml.cs`
  应用启动代码所在位置。负责：
  - 设置应用的关闭模式
  - 创建隐藏控制窗口 `MainWindow`
  - 让应用在后台持续存活

### 3. 控制层

- `src/LockScreen.App/MainWindow.xaml`
  一个几乎不可见的隐藏窗口。
- `src/LockScreen.App/MainWindow.xaml.cs`
  当前 MVP 的控制核心。负责：
  - 隐藏主窗口
  - 取得窗口句柄
  - 注册/注销全局快捷键
  - 接收 `WM_HOTKEY`
  - 在收到热键后打开锁屏窗口

### 4. 锁屏界面层

- `src/LockScreen.App/LockScreenWindow.xaml`
  锁屏 UI 定义。负责：
  - 全屏显示
  - 渐变背景
  - PIN 输入框
  - 错误提示和解锁按钮
- `src/LockScreen.App/LockScreenWindow.xaml.cs`
  锁屏交互逻辑。负责：
  - 焦点进入 PIN 输入框
  - 处理回车解锁
  - 校验 PIN
  - 拦截普通关闭行为

## 运行链路

从程序启动到解锁，当前实现链路如下：

```text
启动应用
  -> App.OnStartup
  -> 创建 MainWindow
  -> MainWindow 加载后立即隐藏
  -> 注册 Ctrl+Alt+L 全局快捷键

用户按下 Ctrl+Alt+L
  -> Windows 发送 WM_HOTKEY
  -> MainWindow.WndProc 收到消息
  -> 调用 ShowLockScreen()
  -> 创建并显示 LockScreenWindow

用户输入 PIN
  -> 点击“解锁”或按 Enter
  -> LockScreenWindow.TryUnlock()
  -> PIN 正确则关闭锁屏窗口
  -> 程序继续后台常驻
```

## 实现原理

## 1. 为什么需要一个隐藏主窗口

虽然程序看起来像“后台程序”，但当前实现仍然保留了一个隐藏的 `MainWindow`。原因是：

- Win32 的 `RegisterHotKey` 需要绑定到一个窗口句柄
- WPF 的窗口最容易提供稳定句柄
- 因此使用一个不可见窗口充当控制中心，是实现成本最低、最稳定的方案

这个隐藏窗口不负责展示业务 UI，只负责：
- 生命周期托管
- 快捷键注册
- 收消息
- 打开锁屏页

## 2. 为什么 `ShutdownMode` 用 `OnExplicitShutdown`

默认情况下，WPF 关闭主窗口时，整个应用可能跟着退出。  
但当前架构里，`MainWindow` 会在启动后立即隐藏，如果使用默认关闭模式，生命周期会不够稳定。

因此在 `App.xaml.cs` 中将：

- `ShutdownMode = ShutdownMode.OnExplicitShutdown`

这样做的意义是：
- 应用不会因为某个窗口关闭而自动退出
- 锁屏窗口关闭后，后台监听仍能继续
- 后续增加托盘、设置页时也更容易管理生命周期

## 3. 全局快捷键是怎么工作的

当前全局快捷键逻辑在 `MainWindow.xaml.cs` 中实现。

主要步骤：
- 通过 `WindowInteropHelper(this).Handle` 拿到原生窗口句柄
- 调用 `RegisterHotKey(...)`
- 将 `Ctrl + Alt + L` 注册到当前进程
- 用 `HwndSource.AddHook(...)` 挂接 Win32 消息处理
- 当收到 `WM_HOTKEY` 时，判断是否是目标快捷键
- 命中后调用 `ShowLockScreen()`

这里的关键点是：
- WPF 本身没有直接提供全局热键 API
- 所以需要借助 Win32 API
- 当前方案对 MVP 足够直接，也方便后续封装成 `HotkeyService`

## 4. 锁屏窗口为什么单独拆出来

锁屏界面被放在独立的 `LockScreenWindow` 中，而不是直接在 `MainWindow` 上切 UI。这样做有几个好处：

- 控制逻辑和展示逻辑分离
- 后续替换 UI 风格不会影响热键机制
- 更容易扩展为多种模式
  - 手动锁屏
  - 空闲进入屏保
  - 多显示器锁屏

当前 `LockScreenWindow` 具备这些窗口属性：
- `WindowStyle="None"`
- `WindowState="Maximized"`
- `Topmost="True"`
- `ShowInTaskbar="False"`

这些配置共同保证：
- 窗口无边框
- 全屏显示
- 默认置顶
- 不在任务栏显示普通入口

## 5. PIN 解锁逻辑是怎么工作的

当前解锁逻辑非常简单，目的只是完成 MVP 闭环。

主要流程：
- 锁屏窗口加载时，焦点自动进入 `PasswordBox`
- 用户点击按钮或按下 Enter 时触发 `TryUnlock()`
- 与硬编码的 `UnlockPin = "1234"` 对比
- 正确则设置 `IsUnlockSuccessful = true`，然后关闭窗口
- 错误则清空输入框并显示错误提示

这里引入了一个 `IsUnlockSuccessful` 标记，是为了配合关闭拦截逻辑：
- 默认关闭锁屏窗口会被阻止
- 只有当 PIN 校验通过后，才允许真正关闭

## 6. 为什么要拦截关闭行为

当前锁屏页会在 `OnClosing` 中阻止普通关闭。这样做是为了避免以下情况：
- 用户直接点关闭
- 误触普通退出路径
- 锁屏还没校验就消失

当前逻辑属于最基础版本，只做了简单拦截。它不是严格安全方案，但对 MVP 有两个实际价值：
- 确保交互流程完整
- 为后续“必须认证后才能退出”打基础

## 当前设计取舍

- 保留隐藏的 `MainWindow`，是为了给 `RegisterHotKey` 提供稳定的窗口句柄
- 将锁屏界面独立成 `LockScreenWindow`，是为了让控制层和界面层解耦
- PIN 暂时写死，是为了优先验证主流程而不是提前引入配置系统
- 当前只做单窗口锁屏，是为了先保证主路径稳定，再考虑托盘、多显示器、空闲触发

## 当前限制

- 这是应用层锁屏，不是 Windows 系统级锁屏替代品
- PIN 目前没有做安全存储
- 还没有托盘图标、设置页面和配置持久化
- 还没有覆盖多显示器场景
- 还没有空闲自动锁屏
- 还没有主题系统和个性化内容模块

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

### 5. 结构升级

当功能继续增长后，建议把当前单项目结构拆成更清晰的层次，例如：
- `Views`
- `Services`
- `Models`
- `Infrastructure/Native`

这样后续引入：
- 热键服务
- 配置服务
- 身份校验服务
- 空闲检测服务

会更清晰，也更方便测试。

## 构建

```powershell
dotnet build .\LockScreen.sln
```

## 运行

```powershell
dotnet run --project .\src\LockScreen.App
```
