using System;
using System.Collections.Generic;
using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace FlaUIRecorder.CodeProvider.Common
{
    public static class SelectorBuilder
    {
        private static readonly ControlType[] ClickableControlTypes =
        {
            ControlType.Button,
            ControlType.Hyperlink,
            ControlType.MenuItem,
            ControlType.CheckBox,
            ControlType.RadioButton,
            ControlType.ComboBox,
            ControlType.ListItem,
            ControlType.TabItem,
            ControlType.SplitButton
        };

        public static SelectorInfo Build(AutomationElement element, bool requireEnabled = true)
        {
            var selector = new SelectorInfo();

            try
            {
                selector.RequireEnabled = requireEnabled && IsClickable(element);

                if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))
                    selector.AutomationId = automationId;

                if (element.Properties.Name.TryGetValue(out var name) && !string.IsNullOrEmpty(name))
                    selector.Name = name;

                if (element.Properties.ControlType.TryGetValue(out var controlType))
                    selector.ControlType = controlType;
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            if (!string.IsNullOrEmpty(selector.AutomationId))
            {
                selector.FindMethod = SelectorFindMethod.FindFirstDescendant;
            }
            else if (!string.IsNullOrEmpty(selector.Name) && selector.ControlType.HasValue)
            {
                selector.FindMethod = SelectorFindMethod.FindFirstChild;
            }
            else if (selector.ControlType.HasValue)
            {
                selector.FindMethod = SelectorFindMethod.FindAllChildren;
            }
            else
            {
                selector.FindMethod = SelectorFindMethod.FindFirstDescendant;
            }

            return selector;
        }

        public static bool IsClickable(AutomationElement element)
        {
            if (element == null)
                return false;

            try
            {
                if (!element.Properties.ControlType.TryGetValue(out var controlType))
                    return false;

                foreach (var clickable in ClickableControlTypes)
                {
                    if (controlType == clickable)
                        return true;
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return false;
        }

        public static string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string BuildCSharpCondition(SelectorInfo selector)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(selector.AutomationId))
                parts.Add($"cf.ByAutomationId(\"{EscapeString(selector.AutomationId)}\")");

            if (selector.ControlType.HasValue)
                parts.Add($"cf.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value})");

            if (!string.IsNullOrEmpty(selector.Name))
                parts.Add($"cf.ByName(\"{EscapeString(selector.Name)}\")");

            if (parts.Count == 0)
                return "cf.True()";

            var result = new StringBuilder(parts[0]);
            for (var i = 1; i < parts.Count; i++)
                result.Append($".And({parts[i]})");

            // Returns the body expression (e.g. "cf.ByAutomationId(...)") — caller wraps with lambda prefix
            return result.ToString();
        }

        public static string BuildPowerShellCondition(SelectorInfo selector)
        {
            var parts = new StringBuilder("$uia.ConditionFactory");

            if (!string.IsNullOrEmpty(selector.AutomationId))
                parts.Append($".ByAutomationId(\"{EscapeString(selector.AutomationId)}\")");
            else if (selector.ControlType.HasValue)
                parts.Append($".ByControlType([FlaUI.Core.Definitions.ControlType]::{selector.ControlType.Value})");
            else
                parts.Append(".True()");

            if (!string.IsNullOrEmpty(selector.AutomationId) && selector.ControlType.HasValue)
                parts.Append($".And($uia.ConditionFactory.ByControlType([FlaUI.Core.Definitions.ControlType]::{selector.ControlType.Value}))");

            if (!string.IsNullOrEmpty(selector.Name))
                parts.Append($".And($uia.ConditionFactory.ByName(\"{EscapeString(selector.Name)}\"))");

            return parts.ToString();
        }
    }
}
