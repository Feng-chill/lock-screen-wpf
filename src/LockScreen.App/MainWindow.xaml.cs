using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LockScreen.App;

public partial class MainWindow : Window
{
    private const int HotkeyId = 9001;
    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;
    private const int WmHotkey = 0x0312;

    private HwndSource? _source;
    private LockScreenWindow? _lockScreenWindow;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Hide();

        var helper = new WindowInteropHelper(this);
        _source = HwndSource.FromHwnd(helper.Handle);
        _source?.AddHook(WndProc);

        if (!RegisterHotKey(helper.Handle, HotkeyId, ModControl | ModAlt, 0x4C))
        {
            MessageBox.Show(
                "注册 Ctrl+Alt+L 失败，可能已被其他程序占用。",
                "Lock Screen MVP",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        UnregisterHotKey(handle, HotkeyId);
        _source?.RemoveHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmHotkey && wParam.ToInt32() == HotkeyId)
        {
            ShowLockScreen();
            handled = true;
        }

        return IntPtr.Zero;
    }

    private void ShowLockScreen()
    {
        if (_lockScreenWindow is { IsVisible: true })
        {
            _lockScreenWindow.Activate();
            return;
        }

        _lockScreenWindow = new LockScreenWindow();
        _lockScreenWindow.Owner = this;
        _lockScreenWindow.Closed += (_, _) => _lockScreenWindow = null;
        _lockScreenWindow.Show();
        _lockScreenWindow.Activate();
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
