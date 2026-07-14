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
        [System.Runtime.InteropServices.DllImport(""user32.dll"")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        const int SW_RESTORE = 9;

        static void BringToFront(Process p)
        {
            try
            {
                var hWnd = p.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, SW_RESTORE);
                    SetForegroundWindow(hWnd);
                }
            }
            catch { }
        }

        static Application LaunchOrAttach(string exePath, string arguments = """")
        {
            var processName = Path.GetFileNameWithoutExtension(exePath);

            // First, try to find an already-running process and attach to it.
            var target = FindTargetProcess(processName);
            if (target != null)
            {
                var app = Application.Attach(target);
                BringToFront(target);
                return app;
            }

            // Not running — launch it and wait for a process with a main window.
            // (Launch may start a short-lived launcher; never return that.
            //  Always wait for a live process with a main window.)
            try
            {
                if (string.IsNullOrWhiteSpace(arguments))
                    Application.Launch(exePath);
                else
                    Application.Launch(new ProcessStartInfo(exePath, arguments));
            }
            catch { }

            FlaUI.Core.Tools.Retry.While(
                () =>
                {
                    target = FindTargetProcess(processName);
                    return target == null;
                },
                result => result,
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

        private const string AutomationExecutorClass = @"
    public class StepResult
    {
        public int Number { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }    // ""Passed"" or ""Failed""
        public double DurationSeconds { get; set; }
        public DateTime Timestamp { get; set; }
        public string Error { get; set; }
        public string ScreenshotBase64 { get; set; }
    }

    public class AutomationExecutor
    {
        private int _step, _passed, _failed;
        private readonly DateTime _startTime = DateTime.Now;
        private readonly List<StepResult> _results = new List<StepResult>();

        public int Passed => _passed;
        public int Failed => _failed;
        public DateTime StartTime => _startTime;
        public IReadOnlyList<StepResult> Results => _results.AsReadOnly();

        public void ExecuteStep(string description, Action action, bool continueOnError = true)
        {
            _step++;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.Write($""[{DateTime.Now:HH:mm:ss.fff}] Step {_step,3}: {description} "");

            var result = new StepResult
            {
                Number = _step,
                Description = description,
                Timestamp = DateTime.Now
            };

            try
            {
                action();
                sw.Stop();
                Console.WriteLine($""OK ({sw.Elapsed.TotalSeconds:F1}s)"");
                result.Status = ""Passed"";
                _passed++;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Console.WriteLine($""FAIL ({sw.Elapsed.TotalSeconds:F1}s)"");
                Console.WriteLine($""                          Reason: {ex.Message}"");
                result.Status = ""Failed"";
                result.Error = ex.Message;
                _failed++;
                if (!continueOnError) throw;
            }

            result.DurationSeconds = sw.Elapsed.TotalSeconds;
            _results.Add(result);
        }

        public AutomationElement FindElement(AutomationElement parent, Func<ConditionFactory, ConditionBase> conditionFn, double timeoutSeconds = 5)
        {
            var result = FlaUI.Core.Tools.Retry.While(
                () => parent.FindFirstDescendant(conditionFn),
                e => e == null,
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeSpan.FromMilliseconds(200)).Result;

            if (result == null)
                throw new InvalidOperationException(""Element not found after retry"");

            try { FlaUI.Core.Tools.Retry.While(() => !result.IsEnabled, r => r, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100)); } catch { }
            return result;
        }

        public void SafeClick(AutomationElement element, FlaUI.Core.Input.MouseButton button = FlaUI.Core.Input.MouseButton.Left, bool doubleClick = false)
        {
            if (element == null) return;
            try { FlaUI.Core.Tools.Retry.While(() => !element.IsEnabled, r => r, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100)); } catch { }
            var point = GetElementClickPoint(element);
            if (doubleClick)
                FlaUI.Core.Input.Mouse.DoubleClick(point, button);
            else
                FlaUI.Core.Input.Mouse.Click(point, button);
        }

        public void SafeType(AutomationElement element, string text)
        {
            if (element == null) return;
            try { FlaUI.Core.Tools.Retry.While(() => !element.IsEnabled, r => r, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(100)); } catch { }
            try { element.Focus(); } catch { }
            try { element.AsTextBox()?.Enter(text); } catch { }
        }

        public void SafeDrag(AutomationElement from, AutomationElement to)
        {
            try
            {
                var start = GetElementClickPoint(from);
                FlaUI.Core.Input.Mouse.MoveTo(start);
                FlaUI.Core.Input.Mouse.Down(FlaUI.Core.Input.MouseButton.Left);
                var end = GetElementClickPoint(to);
                FlaUI.Core.Input.Mouse.MoveTo(end);
                FlaUI.Core.Input.Mouse.Up(FlaUI.Core.Input.MouseButton.Left);
            }
            catch (System.Runtime.InteropServices.COMException) { }
        }

        public void PrintSummary()
        {
            var elapsed = (DateTime.Now - _startTime).TotalSeconds;
            Console.WriteLine();
            Console.WriteLine(""========================================"");
            Console.WriteLine($""  Passed: {_passed}   Failed: {_failed}   Duration: {elapsed:F1}s"");
            Console.WriteLine(""========================================"");
            if (_failed > 0) Environment.ExitCode = 1;
        }

        private System.Drawing.Point GetElementClickPoint(AutomationElement element)
        {
            try { return element.GetClickablePoint(); }
            catch
            {
                try
                {
                    var r = element.Properties.BoundingRectangle.Value;
                    if (!r.IsEmpty && r.Width > 0 && r.Height > 0)
                        return new System.Drawing.Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
                }
                catch { }
                return new System.Drawing.Point(0, 0);
            }
        }
    }";

        private const string TestReportClass = @"
    public class TestReport
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<StepResult> Steps { get; set; } = new List<StepResult>();
        public int Passed { get; set; }
        public int Failed { get; set; }

        public void Save(string path, Window window)
        {
            // Capture screenshots for failed steps
            foreach (var step in Steps)
            {
                if (step.Status == ""Failed"" && window != null)
                {
                    try
                    {
                        using var bitmap = window.Capture();
                        using var ms = new MemoryStream();
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        step.ScreenshotBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                    catch { }
                }
            }

            var html = BuildHtml();
            File.WriteAllText(path, html, Encoding.UTF8);
        }

        private string BuildHtml()
        {
            var duration = (EndTime - StartTime).TotalSeconds;
            var sb = new StringBuilder();

            sb.AppendLine(""<!DOCTYPE html>"");
            sb.AppendLine(""<html lang=\""en\"">"");
            sb.AppendLine(""<head>"");
            sb.AppendLine(""<meta charset=\""UTF-8\"">"");
            sb.AppendLine(""<meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">"");
            sb.AppendLine(""<title>FlaUIRecorder Test Report</title>"");
            sb.AppendLine(ReportStyles);
            sb.AppendLine(""</head>"");
            sb.AppendLine(""<body>"");

            // Header
            sb.AppendLine(""<div class=\""header\"">"");
            sb.AppendFormat(""  <h1>FlaUIRecorder Test Report</h1>\n"");
            sb.AppendFormat(""  <p>Generated {0:yyyy-MM-dd HH:mm:ss}</p>\n"", DateTime.Now);
            sb.AppendLine(""</div>"");

            // Summary cards
            sb.AppendLine(""<div class=\""summary\"">"");
            sb.AppendFormat(""  <div class=\""summary-card card-pass\""><div class=\""value\"">{0}</div><div class=\""label\"">Passed</div></div>\n"", Passed);
            sb.AppendFormat(""  <div class=\""summary-card card-fail\""><div class=\""value\"">{0}</div><div class=\""label\"">Failed</div></div>\n"", Failed);
            sb.AppendFormat(""  <div class=\""summary-card card-time\""><div class=\""value\"">{0:F1}s</div><div class=\""label\"">Duration</div></div>\n"", duration);
            sb.AppendLine(""</div>"");

            // Step table
            sb.AppendLine(""<div class=\""table-wrap\"">"");
            sb.AppendLine(""  <table>"");
            sb.AppendLine(""    <thead><tr><th>#</th><th>Description</th><th>Status</th><th>Duration</th><th>Time</th></tr></thead>"");
            sb.AppendLine(""    <tbody>"");

            foreach (var step in Steps)
            {
                var rowClass = step.Status == ""Passed"" ? ""step-pass"" : ""step-fail"";
                var statusIcon = step.Status == ""Passed"" ? ""&#10003;"" : ""&#10007;"";
                var desc = EscapeHtml(step.Description ?? """");
                sb.AppendFormat(""      <tr class=\""{0}\"">\n"", rowClass);
                sb.AppendFormat(""        <td class=\""num\"">{0}</td>\n"", step.Number);
                sb.AppendFormat(""        <td class=\""desc\"">{0}</td>\n"", desc);
                sb.AppendFormat(""        <td class=\""status\""><span class=\""badge\"">{0} {1}</span></td>\n"", statusIcon, step.Status);
                sb.AppendFormat(""        <td class=\""time\"">{0:F1}s</td>\n"", step.DurationSeconds);
                sb.AppendFormat(""        <td class=\""ts\"">{0:HH:mm:ss}</td>\n"", step.Timestamp);
                sb.AppendLine(""      </tr>"");

                if (step.Status == ""Failed"")
                {
                    var errMsg = EscapeHtml(step.Error ?? """");
                    sb.AppendFormat(""      <tr class=\""step-fail-detail\""><td colspan=\""5\"">"");
                    if (!string.IsNullOrEmpty(errMsg))
                        sb.AppendFormat(""<div class=\""step-error\"">{0}</div>"", errMsg);
                    if (!string.IsNullOrEmpty(step.ScreenshotBase64))
                        sb.AppendFormat(""<details class=\""screenshot-detail\""><summary>Screenshot</summary><img src=\""data:image/png;base64,{0}\"" class=\""screenshot-img\"" /></details>"", step.ScreenshotBase64);
                    sb.AppendLine(""</td></tr>"");
                }
            }

            sb.AppendLine(""    </tbody>"");
            sb.AppendLine(""  </table>"");
            sb.AppendLine(""</div>"");

            sb.AppendFormat(""<div class=\""footer\"">FlaUIRecorder &mdash; {0} steps executed in {1:F1}s</div>\n"", Passed + Failed, duration);
            sb.AppendLine(""</body>"");
            sb.AppendLine(""</html>"");

            return sb.ToString();
        }

        private const string ReportStyles = @""
<style>
  * {{ margin: 0; padding: 0; box-sizing: border-box; }}
  body {{ font-family: 'Segoe UI', system-ui, sans-serif; background: #f0f2f5; color: #1a1a2e; }}
  .header {{ background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%); color: #fff; padding: 30px 40px; }}
  .header h1 {{ font-size: 24px; font-weight: 600; }}
  .header p {{ opacity: 0.7; margin-top: 6px; font-size: 14px; }}
  .summary {{ display: flex; gap: 20px; padding: 30px 40px; max-width: 900px; margin: 0 auto; }}
  .summary-card {{ flex: 1; background: #fff; border-radius: 12px; padding: 24px; text-align: center; box-shadow: 0 2px 8px rgba(0,0,0,0.06); }}
  .summary-card .value {{ font-size: 36px; font-weight: 700; }}
  .summary-card .label {{ font-size: 13px; color: #666; margin-top: 4px; text-transform: uppercase; letter-spacing: 1px; }}
  .card-pass .value {{ color: #22c55e; }}
  .card-fail .value {{ color: #ef4444; }}
  .card-time .value {{ color: #3b82f6; }}
  .table-wrap {{ max-width: 900px; margin: 0 auto 40px; padding: 0 40px; }}
  table {{ width: 100%; border-collapse: collapse; background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.06); }}
  th {{ background: #f8f9fa; padding: 12px 16px; text-align: left; font-size: 12px; text-transform: uppercase; letter-spacing: 0.5px; color: #888; border-bottom: 1px solid #e5e7eb; }}
  td {{ padding: 12px 16px; border-bottom: 1px solid #f3f4f6; font-size: 14px; }}
  td.num {{ width: 50px; color: #999; }}
  td.status {{ width: 100px; }}
  td.time {{ width: 70px; text-align: right; font-family: monospace; }}
  td.ts {{ width: 90px; text-align: right; font-size: 12px; color: #999; }}
  .step-pass td {{ }}
  .step-fail td {{ background: #fef2f2; }}
  .step-fail-detail td {{ background: #fef2f2; padding: 0 16px 16px; }}
  .step-pass-detail td {{ padding: 0; }}
  .badge {{ display: inline-block; padding: 3px 10px; border-radius: 20px; font-size: 12px; font-weight: 600; }}
  .step-pass .badge {{ background: #dcfce7; color: #166534; }}
  .step-fail .badge {{ background: #fecaca; color: #991b1b; }}
  .step-error {{ background: #fff; border: 1px solid #fecaca; border-radius: 8px; padding: 12px 16px; font-size: 13px; color: #991b1b; font-family: monospace; white-space: pre-wrap; margin-bottom: 8px; }}
  .screenshot-detail {{ margin-top: 8px; }}
  .screenshot-detail summary {{ cursor: pointer; color: #3b82f6; font-size: 13px; }}
  .screenshot-img {{ max-width: 100%; border-radius: 8px; margin-top: 8px; border: 1px solid #e5e7eb; }}
  .footer {{ text-align: center; padding: 20px; color: #999; font-size: 12px; }}
</style>"";

        private static string EscapeHtml(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace(""&"", ""&amp;"").Replace(""<"", ""&lt;"").Replace("">"", ""&gt;"");
        }
    }";

        // Upgrade path: the recorder runs on FlaUI 5.0 / net7.0-windows.
        // Exported projects default to the same version, with optional legacy 4.0 target.

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
                ? WrapProgramWithScreenshotOnFailure(automationNs, automationType, executable, launchArguments, options.GenerateHtmlReport)
                : BuildStandardProgram(automationNs, automationType, executable, launchArguments, options.GenerateHtmlReport);

            File.WriteAllText(Path.Combine(exportDir, "Program.cs"), screenshotWrapper);

            var upgradeComment = options.IsFlaUI40
                ? $"<!-- Exported for FlaUI {flaUIVersion} on {targetFramework}. Legacy target; review API differences from 5.0. -->"
                : $"<!-- Exported for FlaUI {flaUIVersion} on {targetFramework}. -->";

            var windowsFormsEntry = targetFramework.StartsWith("net7") ? "    <UseWindowsForms>true</UseWindowsForms>\n" : "";

            var csproj = $@"{upgradeComment}
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{targetFramework}</TargetFramework>
{windowsFormsEntry}    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""FlaUI.Core"" Version=""{flaUIVersion}"" />
    <PackageReference Include=""{uiaPackage}"" Version=""{flaUIVersion}"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(Path.Combine(exportDir, $"{projectName}.csproj"), csproj);

            return exportDir;
        }

        private const string NativeImports = @"
        [System.Runtime.InteropServices.DllImport(""user32.dll"")]
        static extern bool SetProcessDpiAwarenessContext(IntPtr ctx);
        static readonly IntPtr DPI_AWARE_PER_MONITOR_V2 = new IntPtr(-4);

        [System.Runtime.InteropServices.DllImport(""kernel32.dll"")]
        static extern IntPtr GetConsoleWindow();
        [System.Runtime.InteropServices.DllImport(""user32.dll"")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_MINIMIZE = 6;

        static void HideConsole()
        {{
            var hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
                ShowWindow(hWnd, SW_MINIMIZE);
        }}";

        private static string BuildStandardProgram(string automationNs, string automationType, string executable, string launchArguments, bool generateHtmlReport)
        {
            var reportCall = generateHtmlReport
                ? "                var report = new RecordedAutomation().Run(window);\r\n                report.Save(\"TestReport.html\", window);\r\n                Process.Start(new ProcessStartInfo(\"TestReport.html\") { UseShellExecute = true });"
                : "                new RecordedAutomation().Run(window);";

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
{NativeImports}
        static void Main()
        {{
            HideConsole();
            try {{ SetProcessDpiAwarenessContext(DPI_AWARE_PER_MONITOR_V2); }} catch {{ }}
            using (var app = LaunchOrAttach(@""{executable}""{launchArguments}))
            using (var automation = new {automationType}())
            {{
                var window = app.GetMainWindow(automation);
{reportCall}
            }}
        }}
    }}
}}";
        }

        private static string WrapProgramWithScreenshotOnFailure(string automationNs, string automationType, string executable, string launchArguments, bool generateHtmlReport)
        {
            var reportCall = generateHtmlReport
                ? "                    var report = new RecordedAutomation().Run(window);\r\n                    report.Save(\"TestReport.html\", window);\r\n                    Process.Start(new ProcessStartInfo(\"TestReport.html\") { UseShellExecute = true });"
                : "                    new RecordedAutomation().Run(window);";

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
{NativeImports}
        static void Main()
        {{
            HideConsole();
            try {{ SetProcessDpiAwarenessContext(DPI_AWARE_PER_MONITOR_V2); }} catch {{ }}
            using (var app = LaunchOrAttach(@""{executable}""{launchArguments}))
            using (var automation = new {automationType}())
            {{
                var window = app.GetMainWindow(automation);
                try
                {{
{reportCall}
                }}
                catch (Exception ex)
                {{
                    var errorsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""errors"");
                    Directory.CreateDirectory(errorsDir);
                    var fileName = Path.Combine(errorsDir, $""error_{DateTime.Now:yyyyMMdd_HHmmss}.png"");
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
                    if (options.ContinueOnError)
                    {
                        body.AppendLine($"exec.ExecuteStep(\"Session {session.StartTime:g}\", () => {{");
                        var sessionCode = SanitizeSessionCode(session.Code);
                        body.AppendLine("    " + sessionCode.Replace("\n", "\n    ").TrimEnd());
                        body.AppendLine("});");
                    }
                    else
                    {
                        body.AppendLine(SanitizeSessionCode(session.Code));
                    }
                }
                else if (session.Actions != null && session.Actions.Count > 0)
                {
                    body.AppendLine(GenerateFromActions(session.Actions, usePageObjects, options.ContinueOnError));
                }
                else
                {
                    body.AppendLine("// (no recorded actions)");
                }

                body.AppendLine();
            }

            var bodyText = body.ToString();
            var sb = new StringBuilder();
            sb.AppendLine("using FlaUI.Core;");
            sb.AppendLine("using FlaUI.Core.AutomationElements;");
            sb.AppendLine("using FlaUI.Core.Conditions;");
            sb.AppendLine("using FlaUI.Core.Definitions;");
            sb.AppendLine("using FlaUI.Core.Input;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Diagnostics;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            if (usePageObjects)
                sb.AppendLine("using RecordedAutomation.Pages;");
            sb.AppendLine();
            sb.AppendLine("namespace RecordedAutomation");
            sb.AppendLine("{");
            AppendIndentedLines(sb, AutomationExecutorClass, "    ");
            sb.AppendLine();
            AppendIndentedLines(sb, TestReportClass, "    ");
            sb.AppendLine();
            sb.AppendLine("    public class RecordedAutomation");
            sb.AppendLine("    {");
            AppendIndentedLines(sb, SafeClickCodeGenerator.CSharpStaticHelperMethod, "        ");
            sb.AppendLine();
            sb.AppendLine("        public TestReport Run(Window window)");
            sb.AppendLine("        {");
            sb.AppendLine("            var exec = new AutomationExecutor();");
            sb.AppendLine("            void W() { System.Threading.Thread.Sleep(500); }");
            sb.AppendLine("            void WaitForMenuAnimation() { System.Threading.Thread.Sleep(500); }");
            sb.AppendLine();
            AppendIndentedLines(sb, bodyText, "            ");
            sb.AppendLine();
            sb.AppendLine("            exec.PrintSummary();");
            sb.AppendLine("            return new TestReport");
            sb.AppendLine("            {");
            sb.AppendLine("                StartTime = exec.StartTime,");
            sb.AppendLine("                EndTime = DateTime.Now,");
            sb.AppendLine("                Passed = exec.Passed,");
            sb.AppendLine("                Failed = exec.Failed,");
            sb.AppendLine("                Steps = exec.Results.ToList()");
            sb.AppendLine("            };");
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
            sanitized = Regex.Replace(sanitized, @"(\w+)\.Enter\(", "$1.AsTextBox()?.Enter(");
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

        private static string GenerateFromActions(IReadOnlyList<RecordedAction> actions, bool usePageObjects, bool continueOnError = false)
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

                if (action.Type == ActionType.Comment)
                {
                    if (!string.IsNullOrEmpty(action.Comment))
                        sb.AppendLine("// " + action.Comment);
                    continue;
                }

                // For Wait actions, generate directly without executor wrapper
                if (action.Type == ActionType.Wait)
                {
                    sb.AppendLine($"exec.ExecuteStep(\"Wait {action.WaitDurationMs}ms\", () => {{");
                    if (action.WaitDurationMs <= 0)
                        sb.AppendLine("    FlaUI.Core.Tools.Retry.While(() => false, r => r, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1));");
                    else
                        sb.AppendLine($"    System.Threading.Thread.Sleep({action.WaitDurationMs});");
                    sb.AppendLine($"}}, {continueOnError.ToString().ToLowerInvariant()});");
                    sb.AppendLine();
                    continue;
                }

                // Build step description
                var desc = GetStepDescription(action);
                sb.AppendLine($"exec.ExecuteStep(\"{desc}\", () => {{");

                // Generate the inner action code
                var innerSb = new StringBuilder();
                switch (action.Type)
                {
                    case ActionType.KeyPress:
                        if (!string.IsNullOrEmpty(action.KeyName))
                            innerSb.AppendLine(KeyPressCodeGenerator.BuildCSharpKeyPress(action.KeyName));
                        break;

                    case ActionType.Click:
                    case ActionType.RightClick:
                    case ActionType.DoubleClick:
                    case ActionType.TextInput:
                    case ActionType.Assert:
                    case ActionType.Drag:
                    case ActionType.Scroll:
                    case ActionType.HoverStay:
                        AppendElementAction(innerSb, action, existingVariables, elementKeyToVariable, ref retryCommentAdded, usePageObjects, currentPageClass);
                        break;
                }

                AppendIndentedLines(sb, innerSb.ToString(), "    ");
                sb.AppendLine($"}}, {continueOnError.ToString().ToLowerInvariant()});");
                sb.AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        private static string GetStepDescription(RecordedAction action)
        {
            var label = GetActionDescription(action);
            switch (action.Type)
            {
                case ActionType.Click: return $"Click '{label}'";
                case ActionType.RightClick: return $"Right-click '{label}'";
                case ActionType.DoubleClick: return $"Double-click '{label}'";
                case ActionType.TextInput: return $"Enter '{action.TextValue ?? ""}' into '{label}'";
                case ActionType.KeyPress: return $"Press key '{action.KeyName}'";
                case ActionType.Assert: return $"Assert '{label}' equals '{action.ExpectedValue ?? ""}'";
                case ActionType.Drag: return $"Drag '{label}' to '{action.TargetName ?? action.TargetAutomationId ?? "target"}'";
                case ActionType.Scroll: return $"Scroll on '{label}'";
                case ActionType.HoverStay: return $"Hover on '{label}' for {action.HoverDurationMs}ms";
                default: return action.Type.ToString();
            }
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
                var condition = SelectorBuilder.BuildCSharpCondition(selector);

                // BuildCSharpCondition returns "cf.ByAutomationId(...)" — wrap as lambda
                sb.AppendLine($"var {variableName} = exec.FindElement(window, cf => {condition});");
            }

            if (usePageObjects && !string.IsNullOrEmpty(currentPageClass) && action.Type == ActionType.Click)
            {
                sb.AppendLine($"new Pages.{currentPageClass}(window).Click{ToPropertyName(variableName)}();");
                return;
            }

            switch (action.Type)
            {
                case ActionType.Click:
                    sb.AppendLine($"exec.SafeClick({variableName});");
                    if (string.Equals(action.ControlType, ControlType.MenuItem.ToString(), StringComparison.OrdinalIgnoreCase))
                        sb.AppendLine("W();");
                    break;

                case ActionType.RightClick:
                    sb.AppendLine($"exec.SafeClick({variableName}, FlaUI.Core.Input.MouseButton.Right);");
                    break;

                case ActionType.DoubleClick:
                    sb.AppendLine($"exec.SafeClick({variableName}, doubleClick: true);");
                    break;

                case ActionType.TextInput:
                    sb.AppendLine($"exec.SafeType({variableName}, \"{SelectorBuilder.EscapeString(action.TextValue ?? string.Empty)}\");");
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
                    sb.AppendLine($"System.Threading.Thread.Sleep({action.HoverDurationMs});");
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
                var fromCondition = SelectorBuilder.BuildCSharpCondition(BuildSelectorFromAction(action));
                sb.AppendLine($"var {fromVar} = exec.FindElement(window, cf => {fromCondition});");
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
                var toCondition = SelectorBuilder.BuildCSharpCondition(BuildSelectorFromAction(targetAction));
                sb.AppendLine($"var {toVar} = exec.FindElement(window, cf => {toCondition});");
            }

            sb.AppendLine($"exec.SafeDrag({fromVar}, {toVar});");
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
            string label = selector.Description ?? selector.ControlType?.ToString() ?? "element";

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                findExpression =
                    $"{parentVariableName}.FindAllChildren(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";
            }
            else
            {
                var condition = SelectorBuilder.BuildCSharpCondition(selector);
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                findExpression = $"{parentVariableName}.{findMethod}(cf => {condition})";
            }

            sb.AppendLine($"// Find '{label}'");
            sb.AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");
            sb.AppendLine($"    () => {findExpression},");
            sb.AppendLine("    e => e == null,");
            sb.AppendLine("    TimeSpan.FromSeconds(5),");
            sb.AppendLine("    TimeSpan.FromMilliseconds(200)).Result;");
        }

        private static void AppendFindExpression(StringBuilder sb, string parentVariableName, SelectorInfo selector, string indent)
        {
            string findExpression;
            string label = selector.Description ?? selector.ControlType?.ToString() ?? "element";

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                findExpression =
                    $"{parentVariableName}.FindAllChildren(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";
            }
            else
            {
                var condition = SelectorBuilder.BuildCSharpCondition(selector);
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                findExpression = $"{parentVariableName}.{findMethod}(cf => {condition})";
            }

            sb.AppendLine($"{indent}// Find '{label}'");
            sb.AppendLine($"{indent}FlaUI.Core.Tools.Retry.While(");
            sb.AppendLine($"{indent}    () => {findExpression},");
            sb.AppendLine($"{indent}    e => e == null,");
            sb.AppendLine($"{indent}    TimeSpan.FromSeconds(5),");
            sb.AppendLine($"{indent}    TimeSpan.FromMilliseconds(200)).Result;");
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
