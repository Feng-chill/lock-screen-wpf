# Lock Screen WPF

一个基于 WPF 和 .NET 8 的个性化锁屏程序示例。

当前版本是 MVP，已具备以下能力：
- 程序启动后后台常驻
- 注册全局快捷键 `Ctrl+Alt+L`
- 按下快捷键后打开全屏锁屏界面
- 使用本地 PIN 解锁

## 运行方式

```powershell
dotnet run --project .\src\LockScreen.App
```

默认 PIN：`1234`

## 项目结构

```text
src/LockScreen.App/
  App.xaml
  MainWindow.xaml
  LockScreenWindow.xaml
```

## 文档

- 英文开发文档：[docs/development.md](./docs/development.md)
- 中文开发文档：[docs/development.zh-CN.md](./docs/development.zh-CN.md)
