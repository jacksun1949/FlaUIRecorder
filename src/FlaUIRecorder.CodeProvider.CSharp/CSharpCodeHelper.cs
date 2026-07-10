using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System;
using System.Text.RegularExpressions;

namespace FlaUIRecorder.CodeProvider.CSharp
{
    public static class CSharpCodeHelper
    {
        private static readonly ControlType[] ControlTypesWithSuffix =
        {
            ControlType.Button,
            ControlType.Hyperlink,
            ControlType.MenuItem,
            ControlType.CheckBox,
            ControlType.RadioButton,
            ControlType.ComboBox,
            ControlType.ListItem,
            ControlType.TabItem,
            ControlType.SplitButton,
            ControlType.Edit,
            ControlType.Text
        };

        public static string GetVariableName(AutomationElement element, AutomationElement parent = null)
        {
            try
            {
                var controlTypeName = GetControlTypeName(element);
                string baseName = null;

            if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))
                baseName = ToCamelCaseIdentifier(SanitizeIdentifier(automationId));

            if (string.IsNullOrEmpty(baseName)
                && element.Properties.Name.TryGetValue(out var rawName)
                && !string.IsNullOrEmpty(rawName))
            {
                baseName = ToCamelCaseIdentifier(SanitizeNamePart(rawName));
            }

            if (string.IsNullOrEmpty(baseName))
                return ToCamelCaseIdentifier(controlTypeName);

            if (ShouldAppendControlTypeSuffix(element, baseName, controlTypeName))
                return baseName + controlTypeName;

            return baseName;
            }
            catch (System.Runtime.InteropServices.COMException) { return "element"; }
            catch (InvalidOperationException) { return "element"; }
        }

        public static string SanitizeIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);
        }

        private static string GetControlTypeName(AutomationElement element)
        {
            if (element.Properties.ControlType.TryGetValue(out var controlType))
                return controlType.ToString();

            return "Item";
        }

        private static bool ShouldAppendControlTypeSuffix(AutomationElement element, string baseName, string controlTypeName)
        {
            if (!element.Properties.ControlType.TryGetValue(out var controlType))
                return false;

            if (Array.IndexOf(ControlTypesWithSuffix, controlType) < 0)
                return false;

            return !baseName.EndsWith(controlTypeName, StringComparison.OrdinalIgnoreCase);
        }

        private static string ToCamelCaseIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Length == 1)
                return value.ToLowerInvariant();

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static string SanitizeNamePart(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sanitized = Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);
            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return sanitized;
        }
    }
}
