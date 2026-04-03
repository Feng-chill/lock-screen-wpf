using System.Windows;

namespace LockScreen.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var controllerWindow = new MainWindow();
        MainWindow = controllerWindow;
        controllerWindow.Show();
    }
}
