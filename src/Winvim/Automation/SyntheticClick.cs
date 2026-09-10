using System.Windows;
using Winvim.Native;

namespace Winvim.Automation;

/// <summary>Last-resort fallback for elements that expose no invokable UI Automation pattern.</summary>
internal static class SyntheticClick
{
    public static void ClickCenter(Rect screenBounds)
    {
        int x = (int)(screenBounds.X + screenBounds.Width / 2);
        int y = (int)(screenBounds.Y + screenBounds.Height / 2);

        NativeMethods.SetCursorPos(x, y);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
    }
}
