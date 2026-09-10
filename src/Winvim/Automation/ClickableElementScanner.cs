using System.Windows.Automation;

namespace Winvim.Automation;

/// <summary>
/// Walks the UI Automation tree of the foreground window and collects elements that
/// can be "clicked" — anything exposing Invoke, Toggle, SelectionItem or ExpandCollapse.
/// </summary>
public static class ClickableElementScanner
{
    // Caps worst-case scan time on windows with very deep automation trees (e.g. browsers).
    private const int MaxElements = 400;

    public static IReadOnlyList<ClickableElement> Scan(IntPtr windowHandle)
    {
        var results = new List<ClickableElement>();

        AutomationElement? root;
        try
        {
            root = AutomationElement.FromHandle(windowHandle);
        }
        catch
        {
            return results;
        }

        if (root is null) return results;

        var condition = new OrCondition(
            new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true),
            new PropertyCondition(AutomationElement.IsTogglePatternAvailableProperty, true),
            new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true),
            new PropertyCondition(AutomationElement.IsExpandCollapsePatternAvailableProperty, true));

        AutomationElementCollection found;
        try
        {
            found = root.FindAll(TreeScope.Descendants, condition);
        }
        catch
        {
            return results;
        }

        foreach (AutomationElement element in found)
        {
            if (results.Count >= MaxElements) break;

            System.Windows.Rect bounds;
            bool offscreen;
            try
            {
                bounds = element.Current.BoundingRectangle;
                offscreen = element.Current.IsOffscreen;
            }
            catch
            {
                // Element became stale mid-scan (e.g. tree changed) — skip it.
                continue;
            }

            if (offscreen || bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0)
                continue;

            results.Add(new ClickableElement(element, bounds));
        }

        return results;
    }
}
