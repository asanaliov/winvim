using System.Windows.Automation;

namespace Winvim.Automation;

/// <summary>Fires the most appropriate UI Automation pattern for a target, falling back to a real click.</summary>
public static class ElementInvoker
{
    public static void Invoke(ClickableElement target)
    {
        var element = target.Element;

        if (element.TryGetCurrentPattern(InvokePattern.Pattern, out var invokeObj) && invokeObj is InvokePattern invoke)
        {
            invoke.Invoke();
            return;
        }

        if (element.TryGetCurrentPattern(TogglePattern.Pattern, out var toggleObj) && toggleObj is TogglePattern toggle)
        {
            toggle.Toggle();
            return;
        }

        if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionObj) && selectionObj is SelectionItemPattern selection)
        {
            selection.Select();
            return;
        }

        if (element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var expandObj) && expandObj is ExpandCollapsePattern expand)
        {
            if (expand.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
                expand.Expand();
            else
                expand.Collapse();
            return;
        }

        SyntheticClick.ClickCenter(target.ScreenBounds);
    }
}
