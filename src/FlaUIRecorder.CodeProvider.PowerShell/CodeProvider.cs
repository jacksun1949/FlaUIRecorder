using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.CodeProvider.Common.Internals;

namespace FlaUIRecorder.CodeProvider.PowerShell
{
    [CodeProviderName("PowerShell")]
    public class CodeProvider : CodeProviderCore
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly VariableList _existingVariables = new VariableList();
        private bool _retryCommentAdded;

        public CodeProvider(CodeProviderArguments args)
            : base(args)
        {
            var executable = args?.TargetProcess?.MainModule.FileName;
            _builder.AppendLine(AddPowerShellHeader(executable));

            _existingVariables.Add("window", GetMainWindow());
            _existingVariables.Add("desktop", Automation.GetDesktop());
        }

        protected override bool MenuWaitHelperAdded { get; set; }

        public override void AddComment(string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
                AppendLine($"# {comment}");
        }

        public override void Click(AutomationElement element)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"if ($null -eq ${variable.Name}) {{ throw \"Could not find element '{GetElementLabel(element)}'\" }}");
            AppendLine($"${variable.Name}.Click()");
            AppendMenuWaitIfNeeded(element);
        }

        public override void RightClick(AutomationElement element)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"if ($null -eq ${variable.Name}) {{ throw \"Could not find element '{GetElementLabel(element)}'\" }}");
            AppendLine($"${variable.Name}.RightClick()");
            AppendMenuWaitIfNeeded(element);
        }

        public override void TextInput(AutomationElement element, string text)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"if ($null -eq ${variable.Name}) {{ throw \"Could not find element '{GetElementLabel(element)}'\" }}");
            AppendLine($"${variable.Name}.Enter(\"{SelectorBuilder.EscapeString(text)}\")");
        }

        public override void AssertValue(AutomationElement element, string expected)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"if ($null -eq ${variable.Name}) {{ throw \"Could not find element '{GetElementLabel(element)}'\" }}");
            AppendLine($"if (\"{SelectorBuilder.EscapeString(expected)}\" -ne ${variable.Name}.Text) {{ throw \"Assertion failed\" }}");
        }

        public override void Wait(int durationMs)
        {
            if (durationMs <= 0)
                AppendLine("Start-Sleep -Milliseconds 1 # Wait for input processing");
            else
                AppendLine($"Start-Sleep -Milliseconds {durationMs}");
        }

        public override void KeyPress(string keyName)
        {
            AppendLine($"[FlaUI.Core.Input.Keyboard]::Press([FlaUI.Core.WindowsAPI.VirtualKeyShort]::{keyName})");
        }

        public override string Generate()
        {
            _builder.AppendLine(AddPowerShellFooter());
            return _builder.ToString();
        }

        protected override Variable BuildPathToParent(AutomationElement element)
        {
            return BuildPathToParentCore(element, new HashSet<AutomationElement>(), _existingVariables, CreateVariableName, AppendRetryCommentIfNeeded);
        }

        protected override void AppendFindStatement(string variableName, string parentVariableName, SelectorInfo selector)
        {
            var condition = SelectorBuilder.BuildPowerShellCondition(selector);

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                AppendLine($"${variableName} = (${parentVariableName}.FindAllChildren($uia.ConditionFactory.ByControlType([FlaUI.Core.Definitions.ControlType]::{selector.ControlType.Value})))[{selector.ChildIndex}]");
            }
            else
            {
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                AppendLine($"${variableName} = ${parentVariableName}.{findMethod}({condition})");
            }
        }

        protected override void AppendNullCheck(string variableName, SelectorInfo selector)
        {
            AppendLine($"if ($null -eq ${variableName}) {{ throw \"Could not find element '{selector.Description}'\" }}");
        }

        protected override string FormatMenuWaitHelper()
        {
            return "function WaitForMenuAnimation { Start-Sleep -Milliseconds 500 }";
        }

        protected override string FormatMenuWaitStatement()
        {
            return "WaitForMenuAnimation";
        }

        protected override void AppendLine(string line)
        {
            _builder.AppendLine(line);
        }

        private string CreateVariableName(AutomationElement element)
        {
            var parent = GetParentOrMainWindow(element);
            var elementName = PowerShellCodeHelper.GetVariableName(element, parent);
            string suffix;

            if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))
            {
                suffix = SanitizeIdentifier(automationId);
                if (string.IsNullOrEmpty(suffix))
                    suffix = "_" + GetVariableIdentifier(elementName);
            }
            else
            {
                suffix = "_" + GetVariableIdentifier(elementName);
            }

            if (_existingVariables.Any(v => string.Equals(v.Name, elementName + suffix, StringComparison.OrdinalIgnoreCase)))
                suffix = "_" + GetVariableIdentifier(elementName);

            return elementName + suffix;
        }

        private void AppendRetryCommentIfNeeded()
        {
            if (_retryCommentAdded)
                return;

            _retryCommentAdded = true;
            AppendLine("# Tip: For dynamic UIs, retry Find* calls until the element appears or a timeout elapses.");
        }

        private string GetVariableIdentifier(string variableName)
        {
            var amount = _existingVariables.Count(v => v.Name.StartsWith(variableName, StringComparison.OrdinalIgnoreCase) && v.Name.Length > variableName.Length && Char.IsDigit(v.Name[variableName.Length]));
            return (amount + 1).ToString();
        }

        private static string GetElementLabel(AutomationElement element)
        {
            if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))
                return automationId;
            if (element.Properties.Name.TryGetValue(out var name) && !string.IsNullOrEmpty(name))
                return name;
            if (element.Properties.ControlType.TryGetValue(out var controlType))
                return controlType.ToString();
            return "element";
        }

        public override void RecordAction(string automationId, string name, string controlType, string actionType, string comment)
        {
        }

        private string AddPowerShellHeader(string executable)
        {
            const string ExecutableTemplate = "<Path to executable>";
            if (string.IsNullOrEmpty(executable)) executable = ExecutableTemplate;

            var retVal =    @"Add-Type -Path '<Path to>\FlaUI.Core.dll'" + Environment.NewLine
                         +  @"Add-Type -Path '<Path to>\FlaUI.UIA3.dll'" + Environment.NewLine
                         + $@"$app = [FlaUI.Core.Application]::Launch('{executable}')" + Environment.NewLine
                         +  @"$uia = New-Object FlaUI.UIA3.UIA3Automation" + Environment.NewLine
                         +  @"$window = $app.GetMainWindow($uia)" + Environment.NewLine
                         +  @"# --------- [ real test starts here ] ---------" + Environment.NewLine
                         + Environment.NewLine;
            return retVal;
        }

        private string AddPowerShellFooter()
        {
            var retVal = Environment.NewLine + Environment.NewLine
                         + @"# --------- [ real test ends here ] ---------" + Environment.NewLine
                         + @"Start-Sleep -m 3000" + Environment.NewLine
                         + @"$uia.Dispose()" + Environment.NewLine
                         + @"$app.Dispose()";

            return retVal;
        }
    }
}
