# Development Notes

## Overview

This project is a Windows desktop lock screen application based on:
- .NET 8
- WPF
- Win32 global hotkey registration

The current goal is to keep the codebase small while iterating from MVP to a configurable personalized lock screen.

## Current Behavior

1. App starts and stays alive in the background.
2. `MainWindow` acts as an invisible controller window.
3. The controller registers `Ctrl+Alt+L` with `RegisterHotKey`.
4. When the hotkey is pressed, `LockScreenWindow` opens in full-screen mode.
5. The user enters the local PIN to unlock.

## Key Files

- `src/LockScreen.App/App.xaml.cs`
  Application startup and explicit shutdown behavior.
- `src/LockScreen.App/MainWindow.xaml.cs`
  Hidden controller window and global hotkey registration.
- `src/LockScreen.App/LockScreenWindow.xaml`
  MVP lock screen UI.
- `src/LockScreen.App/LockScreenWindow.xaml.cs`
  Unlock logic and close interception.

## Design Choices

- `MainWindow` is hidden instead of removed entirely so the app has a stable window handle for `RegisterHotKey`.
- The lock screen is a dedicated full-screen window so UI work stays isolated from controller logic.
- The PIN is hard-coded in the MVP to keep the first version easy to test.

## Limitations

- This is an app-level lock screen, not a replacement for the Windows system lock screen.
- PIN storage is not secure yet.
- There is no tray icon, settings page, or persistence layer yet.
- Multi-monitor coverage is not implemented yet.

## Recommended Next Steps

### 1. Tray Support

Add a tray icon so the app can:
- exit cleanly
- expose status
- open settings

### 2. Configurable PIN

Move the hard-coded PIN into a local configuration file or protected storage.

Suggested additions:
- `Models/AppConfig.cs`
- `Services/ConfigService.cs`
- `Services/AuthService.cs`

### 3. Personalization

Add content modules to the lock screen:
- clock
- wallpaper rotation
- quote panel
- to-do summary
- weather

### 4. Idle Trigger

Use `GetLastInputInfo` to enter lock screen mode after user inactivity.

## Build

```powershell
dotnet build .\LockScreen.sln
```

## Run

```powershell
dotnet run --project .\src\LockScreen.App
```
