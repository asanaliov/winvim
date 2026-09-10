using System.Windows;
using System.Windows.Automation;

namespace Winvim.Automation;

/// <summary>An interactive element found in the foreground window, with its on-screen bounds.</summary>
public sealed record ClickableElement(AutomationElement Element, Rect ScreenBounds);
