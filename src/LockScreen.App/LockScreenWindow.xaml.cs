using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LockScreen.App;

public partial class LockScreenWindow : Window
{
    private const string UnlockPin = "1234";

    public LockScreenWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => PinInput.Focus();
    }

    private bool IsUnlockSuccessful { get; set; }

    private void OnUnlockClick(object sender, RoutedEventArgs e)
    {
        TryUnlock();
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        HintText.Text = string.Empty;
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape or Key.System)
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            TryUnlock();
            e.Handled = true;
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!IsUnlockSuccessful)
        {
            e.Cancel = true;
            HintText.Text = "请先输入正确 PIN。";
            PinInput.Focus();
        }

        base.OnClosing(e);
    }

    private void TryUnlock()
    {
        if (PinInput.Password != UnlockPin)
        {
            PinInput.Clear();
            HintText.Text = "PIN 错误，请重试。";
            PinInput.Focus();
            return;
        }

        IsUnlockSuccessful = true;
        Close();
    }
}
