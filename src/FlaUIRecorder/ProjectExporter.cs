using FlaUI.Core;
using FlaUIRecorder.Internal;
using System;
using System.IO;
using System.Text;

namespace FlaUIRecorder
{
    public static class ProjectExporter
    {
        public static string Export(RecorderProject project, string exportDir, string projectName)
        {
            Directory.CreateDirectory(exportDir);

            var automationNs = project.AutomationType == AutomationType.UIA2 ? "FlaUI.UIA2" : "FlaUI.UIA3";
            var automationType = project.AutomationType == AutomationType.UIA2 ? "UIA2Automation" : "UIA3Automation";
            var uiaPackage = project.AutomationType == AutomationType.UIA2 ? "FlaUI.UIA2" : "FlaUI.UIA3";
            var executable = string.IsNullOrEmpty(project.Executable) ? "<Path to executable>" : project.Executable.Replace("\\", "\\\\");

            var automationCode = BuildAutomationCode(project);
            File.WriteAllText(Path.Combine(exportDir, "RecordedAutomation.cs"), automationCode);

            var programCode = $@"using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using {automationNs};
using System;

namespace RecordedAutomation
{{
    class Program
    {{
        static void Main()
        {{
            using (var app = Application.Launch(@""{executable}""))
            using (var automation = new {automationType}())
            {{
                var window = app.GetMainWindow(automation);
                new RecordedAutomation().Run(window);
            }}
        }}
    }}
}}";
            File.WriteAllText(Path.Combine(exportDir, "Program.cs"), programCode);

            var csproj = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net461</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""FlaUI.Core"" Version=""1.2.0"" />
    <PackageReference Include=""{uiaPackage}"" Version=""1.2.0"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(Path.Combine(exportDir, $"{projectName}.csproj"), csproj);

            return exportDir;
        }

        private static string BuildAutomationCode(RecorderProject project)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using FlaUI.Core.AutomationElements;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace RecordedAutomation");
            sb.AppendLine("{");
            sb.AppendLine("    public class RecordedAutomation");
            sb.AppendLine("    {");
            sb.AppendLine("        public void Run(Window window)");
            sb.AppendLine("        {");
            sb.AppendLine("            void WaitForMenuAnimation() { System.Threading.Thread.Sleep(500); }");
            sb.AppendLine();

            foreach (var session in project.Sessions)
            {
                sb.AppendLine($"            // Session {session.StartTime}");
                if (!string.IsNullOrEmpty(session.Code))
                {
                    foreach (var line in session.Code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                        sb.AppendLine("            " + line);
                }
                else if (session.Actions != null)
                {
                    foreach (var act in session.Actions)
                    {
                        if (act.Type == ActionType.Comment && !string.IsNullOrEmpty(act.Comment))
                            sb.AppendLine($"            // {act.Comment}");
                        else if (act.Type == ActionType.Wait)
                            sb.AppendLine($"            System.Threading.Thread.Sleep({act.WaitDurationMs});");
                        else if (act.Type == ActionType.KeyPress && !string.IsNullOrEmpty(act.KeyName))
                            sb.AppendLine($"            FlaUI.Core.Input.Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.{act.KeyName});");
                        else
                            sb.AppendLine($"            // {act.Type}: {act.Name ?? act.AutomationId}");
                    }
                }
                sb.AppendLine();
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
