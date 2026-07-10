using System;
using System.Text.RegularExpressions;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace FlaUIRecorder.CodeProvider.Common
{
    /// <summary>
    /// Detects Windows Shell / Explorer desktop elements that must not appear
    /// in generated Find* chains under the application window.
    /// </summary>
    public static class ShellElementFilter
    {
        private static readonly Regex DesktopPaneNamePattern = new Regex(
            @"^(桌面|Desktop)\s*\d*$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly string[] ShellClassNames =
        {
            "Progman",
            "WorkerW",
            "Shell_TrayWnd",
            "Shell_SecondaryTrayWnd"
        };

        public static bool IsShellOrDesktopElement(
            AutomationElement element,
            AutomationElement applicationRoot,
            AutomationBase automation)
        {
            if (element == null)
                return false;

            if (automation != null)
            {
                try
                {
                    var desktop = automation.GetDesktop();
                    if (desktop != null && (ReferenceEquals(element, desktop) || element.Equals(desktop)))
                        return true;
                }
                catch
                {
                    // Ignore desktop lookup failures during filtering.
                }
            }

            if (applicationRoot != null && element.Equals(applicationRoot)
                && IsDesktopPane(element))
            {
                return true;
            }

            try
            {
                if (element.Properties.ClassName.TryGetValue(out var className)
                    && !string.IsNullOrEmpty(className))
                {
                    foreach (var shellClass in ShellClassNames)
                    {
                        if (string.Equals(className, shellClass, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return IsDesktopPane(element);
        }

        public static bool IsDesktopPane(AutomationElement element)
        {
            if (element == null)
                return false;

            try
            {
                if (!element.Properties.ControlType.TryGetValue(out var controlType))
                    return false;

                if (controlType != ControlType.Pane)
                    return false;

                if (!element.Properties.Name.TryGetValue(out var name) || string.IsNullOrEmpty(name))
                    return false;

                return DesktopPaneNamePattern.IsMatch(name.Trim());
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return false;
        }
    }
}
