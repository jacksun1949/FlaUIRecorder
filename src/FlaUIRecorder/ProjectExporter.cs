using FlaUI.Core;
using FlaUI.Core.Definitions;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.CodeProvider.CSharp;
using FlaUIRecorder.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlaUIRecorder
{
    public static class ProjectExporter
    {
        private const string MenuWaitHelper = "void WaitForMenuAnimation() { System.Threading.Thread.Sleep(500); }";
        private const string RetryFindComment = CodeProviderCore.RetryFindComment;

        private const string LaunchOrAttachHelper = @"
        static Application LaunchOrAttach(string exePath, string arguments = """")
        {
            // Launch may start a short-lived launcher (common for Electron/Tauri single-instance apps).
            // Never return that Application — always attach to a live process with a main window.
            try
            {
                if (string.IsNullOrWhiteSpace(arguments))
                    Application.Launch(exePath);
                else
                    Application.Launch(new ProcessStartInfo(exePath, arguments));
            }
            catch { }

            var processName = Path.GetFileNameWithoutExtension(exePath);
            Process target = null;
            FlaUI.Core.Tools.Retry.While(
                () =>
                {
                    target = FindTargetProcess(processName);
                    return target == null;
                },
                TimeSpan.FromSeconds(3),
                TimeSpan.FromMilliseconds(150));

            if (target == null)
            {
                throw new InvalidOperationException(
                    $""Could not find a running process named '{processName}' with a main window. "" +
                    ""For single-instance apps, start the application manually first, then re-run."");
            }

            return Application.Attach(target);
        }

        static Process FindTargetProcess(string processName)
        {
            var withWindow = Process.GetProcessesByName(processName)
                .Where(p =>
                {
                    try
                    {
                        p.Refresh();
                        return !p.HasExited && p.MainWindowHandle != IntPtr.Zero;
                    }
                    catch { return false; }
                })
                .OrderByDescending(p =>
                {
                    try { return p.MainWindowHandle.ToInt64(); }
                    catch { return 0L; }
                })
                .ToArray();

            if (withWindow.Length > 0)
                return withWindow[0];

            return Process.GetProcessesByName(processName)
                .Where(p =>
                {
                    try { return !p.HasExited; }
                    catch { return false; }
                })
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();
        }";

        // Upgrade path: the recorder stays on FlaUI 1.2.0 / net461.
        // Exported projects may target FlaUI 4.x / net472 via ExportOptions.
        // API changes between 1.x and 4.x (namespaces, condition builders, retry helpers) require manual review after export.

        public static string Export(RecorderProject project, string exportDir, string projectName, ExportOptions options = null)
        {
            options = options ?? new ExportOptions();
            Directory.CreateDirectory(exportDir);

            if (options.CaptureScreenshotOnFailure)
                Directory.CreateDirectory(Path.Combine(exportDir, "errors"));

            var automationNs = project.AutomationType == AutomationType.UIA2 ? "FlaUI.UIA2" : "FlaUI.UIA3";
            var automationType = project.AutomationType == AutomationType.UIA2 ? "UIA2Automation" : "UIA3Automation";
            var uiaPackage = project.AutomationType == AutomationType.UIA2 ? "FlaUI.UIA2" : "FlaUI.UIA3";
            var flaUIVersion = options.FlaUIVersion;
            var targetFramework = options.TargetFramework;
            var executable = EscapeForCSharpVerbatimString(project.Executable ?? string.Empty);
            if (string.IsNullOrEmpty(project.Executable))
                executable = "<Path to executable>";

            var arguments = EscapeForCSharpVerbatimString(project.Arguments ?? string.Empty);
            var launchArguments = string.IsNullOrEmpty(project.Arguments)
                ? string.Empty
                : $", @\"{arguments}\"";

            var totalActions = project.Sessions?.Sum(s => s.Actions?.Count ?? 0) ?? 0;
            var usePageObjects = options.GeneratePageObjects && totalActions > options.PageObjectActionThreshold;

            var automationCode = BuildAutomationCode(project, usePageObjects, options);
            File.WriteAllText(Path.Combine(exportDir, "RecordedAutomation.cs"), automationCode);

            if (usePageObjects)
                GeneratePageObjectFiles(project, exportDir, options);

            var screenshotWrapper = options.CaptureScreenshotOnFailure
                ? WrapProgramWithScreenshotOnFailure(automationNs, automationType, executable, launchArguments)
                : BuildStandardProgram(automationNs, automationType, executable, launchArguments);

            File.WriteAllText(Path.Combine(exportDir, "Program.cs"), screenshotWrapper);

            var upgradeComment = options.IsFlaUI40
                ? $"<!-- Exported for FlaUI {flaUIVersion} on {targetFramework}. Review API migration from 1.2.0. -->"
                : $"<!-- Exported for FlaUI {flaUIVersion} on {targetFramework} (recorder-compatible). -->";

            var csproj = $@"{upgradeComment}
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{targetFramework}</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""FlaUI.Core"" Version=""{flaUIVersion}"" />
    <PackageReference Include=""{uiaPackage}"" Version=""{flaUIVersion}"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(Path.Combine(exportDir, $"{projectName}.csproj"), csproj);

            return exportDir;
        }

        private static string BuildStandardProgram(string automationNs, string automationType, string executable, string launchArguments)
        {
            return $@"using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using {automationNs};
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace RecordedAutomation
{{
    class Program
    {{
{LaunchOrAttachHelper}
        static void Main()
        {{
            using (var app = LaunchOrAttach(@""{executable}""{launchArguments}))
            using (var automation = new {automationType}())
            {{
                var window = app.GetMainWindow(automation);
                new RecordedAutomation().Run(window);
            }}
        }}
    }}
}}";
        }

        private static string WrapProgramWithScreenshotOnFailure(string automationNs, string automationType, string executable, string launchArguments)
        {
            return $@"using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using {automationNs};
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace RecordedAutomation
{{
    class Program
    {{
{LaunchOrAttachHelper}
        static void Main()
        {{
            using (var app = LaunchOrAttach(@""{executable}""{launchArguments}))
            using (var automation = new {automationType}())
            {{
                var window = app.GetMainWindow(automation);
                try
                {{
                    new RecordedAutomation().Run(window);
                }}
                catch (Exception ex)
                {{
                    var errorsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""errors"");
                    Directory.CreateDirectory(errorsDir);
                    var fileName = Path.Combine(errorsDir, $""error_{{DateTime.Now:yyyyMMdd_HHmmss}}.png"");
                    try {{ window.Capture().Save(fileName); }} catch {{ }}
                    throw new InvalidOperationException($""Automation failed. Screenshot: {{fileName}}"", ex);
                }}
            }}
        }}
    }}
}}";
        }

        private static string BuildAutomationCode(RecorderProject project, bool usePageObjects, ExportOptions options)
        {
            var body = new StringBuilder();

            foreach (var session in project.Sessions ?? Enumerable.Empty<RecordSession>())
            {
                body.AppendLine($"// Session {session.StartTime}");

                if (!string.IsNullOrWhiteSpace(session.Code))
                {
                    body.AppendLine(SanitizeSessionCode(session.Code));
                }
                else if (session.Actions != null && session.Actions.Count > 0)
                {
                    body.AppendLine(GenerateFromActions(session.Actions, usePageObjects));
                }
                else
                {
                    body.AppendLine("// (no recorded actions)");
                }

                body.AppendLine();
            }

            var bodyText = body.ToString();
            var sb = new StringBuilder();
            sb.AppendLine("using FlaUI.Core.AutomationElements;");
            sb.AppendLine("using System;");
            if (usePageObjects)
                sb.AppendLine("using RecordedAutomation.Pages;");
            sb.AppendLine();
            sb.AppendLine("namespace RecordedAutomation");
            sb.AppendLine("{");
            sb.AppendLine("    public class RecordedAutomation");
            sb.AppendLine("    {");
            AppendIndentedLines(sb, SafeClickCodeGenerator.CSharpStaticHelperMethod, "        ");
            sb.AppendLine();
            sb.AppendLine("        public void Run(Window window)");
            sb.AppendLine("        {");
            sb.AppendLine("            " + MenuWaitHelper);

            if (!bodyText.Contains(RetryFindComment))
                sb.AppendLine("            " + RetryFindComment);

            sb.AppendLine();
            AppendIndentedLines(sb, bodyText, "            ");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void GeneratePageObjectFiles(RecorderProject project, string exportDir, ExportOptions options)
        {
            var pagesDir = Path.Combine(exportDir, "Pages");
            Directory.CreateDirectory(pagesDir);

            var allActions = project.Sessions?.SelectMany(s => s.Actions ?? Enumerable.Empty<RecordedAction>()).ToList()
                ?? new List<RecordedAction>();

            var pages = DetectPageBoundaries(allActions);
            foreach (var page in pages)
            {
                var className = page.ClassName;
                var sb = new StringBuilder();
                sb.AppendLine("using FlaUI.Core.AutomationElements;");
                sb.AppendLine("using FlaUI.Core.Definitions;");
                sb.AppendLine("using System;");
                sb.AppendLine();
                sb.AppendLine("namespace RecordedAutomation.Pages");
                sb.AppendLine("{");
                sb.AppendLine($"    public class {className}");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly Window _window;");
                sb.AppendLine();
                sb.AppendLine($"        public {className}(Window window)");
                sb.AppendLine("        {");
                sb.AppendLine("            _window = window;");
                sb.AppendLine("        }");
                sb.AppendLine();

                var elementKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var existingVariables = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "window", "_window" };

                foreach (var action in page.Actions.Where(a => NeedsElement(a)))
                {
                    var key = BuildElementKey(action);
                    if (elementKeys.ContainsKey(key))
                        continue;

                    var varName = CreateUniqueVariableName(action, existingVariables);
                    elementKeys[key] = varName;
                    var selector = BuildSelectorFromAction(action);
                    sb.AppendLine($"        public AutomationElement {ToPropertyName(varName)} =>");
                    AppendFindExpression(sb, "_window", selector, "            ");
                    sb.AppendLine();
                }

                foreach (var action in page.Actions.Where(a => a.Type == ActionType.Click))
                {
                    var key = BuildElementKey(action);
                    if (!elementKeys.TryGetValue(key, out var varName))
                        continue;
                    var methodName = "Click" + ToPropertyName(varName);
                    sb.AppendLine($"        public void {methodName}() {{ RecordedAutomation.SafeClick({ToPropertyName(varName)}); }}");
                }

                sb.AppendLine("    }");
                sb.AppendLine("}");
                File.WriteAllText(Path.Combine(pagesDir, className + ".cs"), sb.ToString());
            }
        }

        private static List<PageSegment> DetectPageBoundaries(IReadOnlyList<RecordedAction> actions)
        {
            var segments = new List<PageSegment>();
            PageSegment current = null;
            var windowIndex = 0;

            foreach (var action in actions)
            {
                if (IsWindowBoundary(action))
                {
                    windowIndex++;
                    current = new PageSegment
                    {
                        ClassName = SanitizePageClassName(action.Name ?? action.AutomationId ?? $"Window{windowIndex}")
                    };
                    segments.Add(current);
                }

                if (current == null)
                {
                    current = new PageSegment { ClassName = "MainWindow" };
                    segments.Add(current);
                }

                current.Actions.Add(action);
            }

            if (segments.Count == 0)
                segments.Add(new PageSegment { ClassName = "MainWindow" });

            return segments;
        }

        private static bool IsWindowBoundary(RecordedAction action)
        {
            return string.Equals(action.ControlType, ControlType.Window.ToString(), StringComparison.OrdinalIgnoreCase)
                || string.Equals(action.ControlType, "Window", StringComparison.OrdinalIgnoreCase);
        }

        private static string SanitizePageClassName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "MainWindow";
            var sanitized = Regex.Replace(name, "[^a-zA-Z0-9_]", "");
            if (sanitized.Length == 0)
                return "MainWindow";
            if (!sanitized.EndsWith("Window", StringComparison.OrdinalIgnoreCase))
                sanitized += "Window";
            return char.ToUpper(sanitized[0]) + sanitized.Substring(1);
        }

        private static string SanitizeSessionCode(string code)
        {
            var sanitized = ReplaceNUnitAssertions(code);
            sanitized = Regex.Replace(
                sanitized,
                @"^\s*void\s+WaitForMenuAnimation\(\)\s*\{[^}]*\}\s*",
                string.Empty,
                RegexOptions.Multiline);
            sanitized = Regex.Replace(sanitized, @"(\w+)\.DragTo\((\w+)\)", "SafeDrag($1, $2)");
            sanitized = sanitized.Replace("element.BoundingRectangle", "element.Properties.BoundingRectangle.Value");
            return sanitized.TrimEnd();
        }

        private static string ReplaceNUnitAssertions(string code)
        {
            return Regex.Replace(
                code,
                @"NUnit\.Framework\.Assert\.AreEqual\(""((?:[^""\\]|\\.)*)"",\s*([^;]+)\);",
                match =>
                {
                    var expected = match.Groups[1].Value;
                    var actualExpression = match.Groups[2].Value.Trim();
                    return
                        "{ var actualValue = " + actualExpression + ";" +
                        " if (actualValue != \"" + expected + "\")" +
                        " throw new InvalidOperationException($\"Expected '" + expected + "' but was '{actualValue}'\"); }";
                });
        }

        private static string GenerateFromActions(IReadOnlyList<RecordedAction> actions, bool usePageObjects)
        {
            var sb = new StringBuilder();
            var existingVariables = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "window" };
            var elementKeyToVariable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var retryCommentAdded = false;
            string currentPageClass = null;

            foreach (var action in actions)
            {
                if (usePageObjects && IsWindowBoundary(action))
                    currentPageClass = SanitizePageClassName(action.Name ?? action.AutomationId ?? "Window");

                switch (action.Type)
                {
                    case ActionType.Comment:
                        if (!string.IsNullOrEmpty(action.Comment))
                            sb.AppendLine("// " + action.Comment);
                        break;

                    case ActionType.Wait:
                        if (action.WaitDurationMs <= 0)
                            sb.AppendLine("FlaUI.Core.Tools.Retry.While(() => false, TimeSpan.FromMilliseconds(1)); // Wait for input processing");
                        else
                            sb.AppendLine($"System.Threading.Thread.Sleep({action.WaitDurationMs});");
                        break;

                    case ActionType.KeyPress:
                        if (!string.IsNullOrEmpty(action.KeyName))
                            sb.AppendLine(KeyPressCodeGenerator.BuildCSharpKeyPress(action.KeyName));
                        break;

                    case ActionType.Click:
                    case ActionType.RightClick:
                    case ActionType.DoubleClick:
                    case ActionType.TextInput:
                    case ActionType.Assert:
                    case ActionType.Drag:
                    case ActionType.Scroll:
                    case ActionType.HoverStay:
                        AppendElementAction(sb, action, existingVariables, elementKeyToVariable, ref retryCommentAdded, usePageObjects, currentPageClass);
                        break;
                }
            }

            return sb.ToString().TrimEnd();
        }

        private static bool NeedsElement(RecordedAction action)
        {
            return action.Type == ActionType.Click || action.Type == ActionType.RightClick
                || action.Type == ActionType.DoubleClick || action.Type == ActionType.TextInput
                || action.Type == ActionType.Assert || action.Type == ActionType.Scroll
                || action.Type == ActionType.HoverStay;
        }

        private static void AppendElementAction(
            StringBuilder sb,
            RecordedAction action,
            HashSet<string> existingVariables,
            Dictionary<string, string> elementKeyToVariable,
            ref bool retryCommentAdded,
            bool usePageObjects,
            string currentPageClass)
        {
            var elementKey = BuildElementKey(action);

            if (!elementKeyToVariable.TryGetValue(elementKey, out var variableName))
            {
                variableName = CreateUniqueVariableName(action, existingVariables);
                elementKeyToVariable[elementKey] = variableName;
                var selector = BuildSelectorFromAction(action);
                var description = GetActionDescription(action);

                if (!retryCommentAdded)
                {
                    sb.AppendLine(RetryFindComment);
                    retryCommentAdded = true;
                }

                AppendFindStatement(sb, variableName, "window", selector);
                sb.AppendLine($"if ({variableName} == null) throw new InvalidOperationException(\"Could not find element '{description}'\");");
            }

            if (usePageObjects && !string.IsNullOrEmpty(currentPageClass) && action.Type == ActionType.Click)
            {
                sb.AppendLine($"new Pages.{currentPageClass}(window).Click{ToPropertyName(variableName)}();");
                return;
            }

            switch (action.Type)
            {
                case ActionType.Click:
                    sb.AppendLine(SafeClickCodeGenerator.BuildCSharpClick(variableName));
                    if (string.Equals(action.ControlType, ControlType.MenuItem.ToString(), StringComparison.OrdinalIgnoreCase))
                        sb.AppendLine("WaitForMenuAnimation();");
                    break;

                case ActionType.RightClick:
                    sb.AppendLine(SafeClickCodeGenerator.BuildCSharpRightClick(variableName));
                    break;

                case ActionType.DoubleClick:
                    sb.AppendLine(SafeClickCodeGenerator.BuildCSharpDoubleClick(variableName));
                    break;

                case ActionType.TextInput:
                    sb.AppendLine($"{variableName}.Enter(\"{SelectorBuilder.EscapeString(action.TextValue ?? string.Empty)}\");");
                    break;

                case ActionType.Assert:
                    var expected = SelectorBuilder.EscapeString(action.ExpectedValue ?? string.Empty);
                    sb.AppendLine("{ var actualValue = " + variableName + ".AsTextBox()?.Text ?? " + variableName + ".Patterns.Value.Pattern.Value;" +
                                  " if (actualValue != \"" + expected + "\")" +
                                  " throw new InvalidOperationException($\"Expected '" + expected + "' but was '{actualValue}'\"); }");
                    break;

                case ActionType.Drag:
                    AppendDragAction(sb, action, existingVariables, elementKeyToVariable, ref retryCommentAdded);
                    break;

                case ActionType.Scroll:
                    var clicks = Math.Max(1, Math.Abs(action.ScrollDelta) / 120);
                    var direction = action.ScrollDelta > 0 ? "true" : "false";
                    sb.AppendLine($"FlaUI.Core.Input.Mouse.Scroll({clicks}, {direction});");
                    break;

                case ActionType.HoverStay:
                    sb.AppendLine($"System.Threading.Thread.Sleep({action.HoverDurationMs}); // Hover-stay");
                    break;
            }
        }

        private static void AppendDragAction(
            StringBuilder sb,
            RecordedAction action,
            HashSet<string> existingVariables,
            Dictionary<string, string> elementKeyToVariable,
            ref bool retryCommentAdded)
        {
            var fromKey = BuildElementKey(action);
            if (!elementKeyToVariable.TryGetValue(fromKey, out var fromVar))
            {
                fromVar = CreateUniqueVariableName(action, existingVariables);
                elementKeyToVariable[fromKey] = fromVar;
                if (!retryCommentAdded) { sb.AppendLine(RetryFindComment); retryCommentAdded = true; }
                AppendFindStatement(sb, fromVar, "window", BuildSelectorFromAction(action));
            }

            var targetAction = new RecordedAction
            {
                AutomationId = action.TargetAutomationId,
                Name = action.TargetName,
                ControlType = action.TargetControlType,
                Type = ActionType.Drag
            };
            var toKey = BuildElementKey(targetAction);
            if (!elementKeyToVariable.TryGetValue(toKey, out var toVar))
            {
                toVar = CreateUniqueVariableName(targetAction, existingVariables);
                elementKeyToVariable[toKey] = toVar;
                AppendFindStatement(sb, toVar, "window", BuildSelectorFromAction(targetAction));
            }

            sb.AppendLine(SafeClickCodeGenerator.BuildCSharpDrag(fromVar, toVar));
        }

        private static string BuildElementKey(RecordedAction action)
        {
            return $"{action.AutomationId ?? ""}|{action.Name ?? ""}|{action.ControlType ?? ""}";
        }

        private static SelectorInfo BuildSelectorFromAction(RecordedAction action)
        {
            var selector = new SelectorInfo();

            if (!string.IsNullOrEmpty(action.AutomationId))
                selector.AutomationId = action.AutomationId;

            if (!string.IsNullOrEmpty(action.Name))
                selector.Name = action.Name;

            if (!string.IsNullOrEmpty(action.ControlType) &&
                Enum.TryParse(action.ControlType, out ControlType controlType))
            {
                selector.ControlType = controlType;
            }

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
                selector.ChildIndex = 0;
            }
            else
            {
                selector.FindMethod = SelectorFindMethod.FindFirstDescendant;
            }

            selector.RequireEnabled = action.Type == ActionType.Click || action.Type == ActionType.RightClick
                || action.Type == ActionType.DoubleClick;
            return selector;
        }

        private static void AppendFindStatement(StringBuilder sb, string variableName, string parentVariableName, SelectorInfo selector)
        {
            string findExpression;

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                findExpression =
                    $"{parentVariableName}.FindAllChildren(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";
            }
            else
            {
                var condition = SelectorBuilder.BuildCSharpCondition(selector);
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                findExpression = $"{parentVariableName}.{findMethod}(e => {condition})";
            }

            sb.AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");
            sb.AppendLine($"    () => {findExpression},");
            sb.AppendLine("    e => e == null,");
            sb.AppendLine("    TimeSpan.FromSeconds(5),");
            sb.AppendLine("    TimeSpan.FromMilliseconds(200));");
        }

        private static void AppendFindExpression(StringBuilder sb, string parentVariableName, SelectorInfo selector, string indent)
        {
            string findExpression;

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                findExpression =
                    $"{parentVariableName}.FindAllChildren(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";
            }
            else
            {
                var condition = SelectorBuilder.BuildCSharpCondition(selector);
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                findExpression = $"{parentVariableName}.{findMethod}(e => {condition})";
            }

            sb.AppendLine($"{indent}FlaUI.Core.Tools.Retry.While(");
            sb.AppendLine($"{indent}    () => {findExpression},");
            sb.AppendLine($"{indent}    e => e == null,");
            sb.AppendLine($"{indent}    TimeSpan.FromSeconds(5),");
            sb.AppendLine($"{indent}    TimeSpan.FromMilliseconds(200));");
        }

        private static string CreateUniqueVariableName(RecordedAction action, HashSet<string> existingVariables)
        {
            var baseName = GetVariableBaseName(action);
            var suffix = !string.IsNullOrEmpty(action.AutomationId)
                ? CSharpCodeHelper.SanitizeIdentifier(action.AutomationId)
                : string.Empty;

            if (string.IsNullOrEmpty(suffix))
                suffix = "1";

            var candidate = baseName + suffix;
            var counter = 1;
            while (existingVariables.Contains(candidate))
            {
                candidate = baseName + suffix + counter;
                counter++;
            }

            existingVariables.Add(candidate);
            return candidate;
        }

        private static string GetVariableBaseName(RecordedAction action)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(action.Name))
            {
                var sanitized = SanitizeNamePart(action.Name);
                if (!string.IsNullOrEmpty(sanitized))
                    parts.Add(sanitized);
            }

            if (!string.IsNullOrEmpty(action.AutomationId))
            {
                var sanitizedId = SanitizeNamePart(action.AutomationId);
                if (!string.IsNullOrEmpty(sanitizedId) &&
                    !parts.Any(p => string.Equals(p, sanitizedId, StringComparison.OrdinalIgnoreCase)))
                {
                    parts.Add(sanitizedId);
                }
            }

            var controlTypeName = string.IsNullOrEmpty(action.ControlType) ? "Item" : action.ControlType;

            if (parts.Count > 0)
            {
                var combined = string.Join(string.Empty, parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
                return char.ToLower(combined[0]) + combined.Substring(1) + controlTypeName;
            }

            return char.ToLower(controlTypeName[0]) + controlTypeName.Substring(1);
        }

        private static string ToPropertyName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return "Element";
            return char.ToUpper(variableName[0]) + variableName.Substring(1);
        }

        private static string SanitizeNamePart(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sanitized = Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);
            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return sanitized.Length > 0 ? char.ToLower(sanitized[0]) + sanitized.Substring(1) : string.Empty;
        }

        private static string GetActionDescription(RecordedAction action)
        {
            if (!string.IsNullOrEmpty(action.AutomationId))
                return action.AutomationId;
            if (!string.IsNullOrEmpty(action.Name))
                return action.Name;
            if (!string.IsNullOrEmpty(action.ControlType))
                return action.ControlType;
            return "element";
        }

        private static void AppendIndentedLines(StringBuilder sb, string text, string indent)
        {
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.AppendLine(indent + line);
            }
        }

        private static string EscapeForCSharpVerbatimString(string value)
        {
            return (value ?? string.Empty).Replace("\"", "\"\"");
        }

        private class PageSegment
        {
            public string ClassName { get; set; }
            public List<RecordedAction> Actions { get; } = new List<RecordedAction>();
        }
    }
}
