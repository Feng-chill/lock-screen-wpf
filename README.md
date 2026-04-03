# Lock Screen WPF

A simple personalized lock screen app built with WPF on .NET 8.

Current version is an MVP:
- Runs in the background after startup
- Registers the global hotkey `Ctrl+Alt+L`
- Opens a full-screen lock screen window
- Unlocks with a local PIN

## Run

```powershell
dotnet run --project .\src\LockScreen.App
```

Default PIN: `1234`

## Project Structure

```text
src/LockScreen.App/
  App.xaml
  MainWindow.xaml
  LockScreenWindow.xaml
```

## Next

See [docs/development.md](./docs/development.md) for implementation notes and planned improvements.
