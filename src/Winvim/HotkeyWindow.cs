using System.Windows;
using System.Windows.Interop;
using Winvim.Native;

namespace Winvim;

/// <summary>
/// Invisible message-only-ish window whose sole job is to own an HWND so we can
/// call RegisterHotKey and pump WM_HOTKEY. It is never shown to the user.
/// </summary>
public sealed class HotkeyWindow : Window
{
    private const int HotkeyId = 0x4000;

    private HwndSource? _source;

    public event Action? HintModeRequested;

    public HotkeyWindow()
    {
        Width = 0;
        Height = 0;
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
        ShowActivated = false;
        ResizeMode = ResizeMode.NoResize;
        Opacity = 0;
        Left = -10000;
        Top = -10000;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        _source = (HwndSource)PresentationSource.FromVisual(this)!;
        _source.AddHook(WndProc);

        NativeMethods.RegisterHotKey(
            _source.Handle,
            HotkeyId,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_F);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HotkeyId)
        {
            HintModeRequested?.Invoke();
            handled = true;
        }

        return IntPtr.Zero;
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_source is not null)
        {
            NativeMethods.UnregisterHotKey(_source.Handle, HotkeyId);
            _source.RemoveHook(WndProc);
        }

        base.OnClosed(e);
    }
}
