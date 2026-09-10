using System.Windows;
using System.Windows.Forms;
using Winvim.Automation;
using Winvim.Native;
using Winvim.Overlay;
using Application = System.Windows.Application;

namespace Winvim;

public partial class App : Application
{
    private HotkeyWindow? _hotkeyWindow;
    private NotifyIcon? _trayIcon;
    private HintOverlayWindow? _overlay;
    private bool _hintModeActive;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _hotkeyWindow = new HotkeyWindow();
        _hotkeyWindow.HintModeRequested += ActivateHintMode;
        _hotkeyWindow.Show();

        CreateTrayIcon();
    }

    private void CreateTrayIcon()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Activate hints (Ctrl+Alt+F)", null, (_, _) => ActivateHintMode());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => Shutdown());

        _trayIcon = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "winvim — Ctrl+Alt+F to click with the keyboard",
            ContextMenuStrip = menu
        };
    }

    private void ActivateHintMode()
    {
        if (_hintModeActive) return;

        var foreground = NativeMethods.GetForegroundWindow();
        if (foreground == IntPtr.Zero) return;

        var elements = ClickableElementScanner.Scan(foreground);
        if (elements.Count == 0) return;

        _hintModeActive = true;
        _overlay = new HintOverlayWindow(elements);
        _overlay.Closed += (_, _) =>
        {
            _hintModeActive = false;
            _overlay = null;
        };
        _overlay.Show();
        _overlay.Activate();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        _hotkeyWindow?.Close();
        base.OnExit(e);
    }
}
