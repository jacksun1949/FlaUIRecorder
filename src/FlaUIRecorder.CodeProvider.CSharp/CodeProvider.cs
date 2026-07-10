using FlaUIRecorder.CodeProvider.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlaUI.Core.AutomationElements;
using FlaUIRecorder.CodeProvider.Common.Internals;

namespace FlaUIRecorder.CodeProvider.CSharp
{
    [CodeProviderName("C#")]
    public class CodeProvider : CodeProviderCore
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly VariableList _existingVariables = new VariableList();
        private bool _retryCommentAdded;

        public CodeProvider(CodeProviderArguments args) : base(args)
        {
            _existingVariables.Add("window", GetMainWindow());
            _existingVariables.Add("desktop", Automation.GetDesktop());
        }

        protected override bool MenuWaitHelperAdded { get; set; }

        public override void AddComment(string comment)
        {
            AppendLine($"// {comment}");
        }

        public override void Click(AutomationElement element)
        {
            var variable = BuildPathToParent(element);
            AppendSafeClickHelperIfNeeded();
            AppendLine($"// Click '{GetElementLabel(element)}'");
            AppendLine(SafeClickCodeGenerator.BuildCSharpClick(variable.Name));
            AppendMenuWaitIfNeeded(element);
        }

        public override void RightClick(AutomationElement element)
        {
            var variable = BuildPathToParent(element);
            AppendSafeClickHelperIfNeeded();
            AppendLine($"// Right-click '{GetElementLabel(element)}'");
            AppendLine(SafeClickCodeGenerator.BuildCSharpRightClick(variable.Name));
            AppendMenuWaitIfNeeded(element);
        }

        public override void DoubleClick(AutomationElement element)
        {
            var variable = BuildPathToParent(element);
            AppendSafeClickHelperIfNeeded();
            AppendLine($"// Double-click '{GetElementLabel(element)}'");
            AppendLine(SafeClickCodeGenerator.BuildCSharpDoubleClick(variable.Name));
        }

        public override void Drag(AutomationElement fromElement, AutomationElement toElement)
        {
            var fromVar = BuildPathToParent(fromElement);
            var toVar = BuildPathToParent(toElement);
            AppendSafeClickHelperIfNeeded();
            AppendLine($"// Drag '{GetElementLabel(fromElement)}' -> '{GetElementLabel(toElement)}'");
            AppendLine(SafeClickCodeGenerator.BuildCSharpDrag(fromVar.Name, toVar.Name));
        }

        public override void Scroll(AutomationElement element, int delta)
        {
            BuildPathToParent(element);
            var clicks = Math.Max(1, Math.Abs(delta) / 120);
            var direction = delta > 0 ? "true" : "false";
            AppendLine($"// Scroll on '{GetElementLabel(element)}'");
            AppendLine($"FlaUI.Core.Input.Mouse.Scroll({clicks}, {direction});");
        }

        public override void HoverStay(AutomationElement element, int durationMs)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"// Hover on '{GetElementLabel(element)}' for {durationMs}ms");
            AppendLine($"System.Threading.Thread.Sleep({durationMs});");
        }

        public override void TextInput(AutomationElement element, string text)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"// Enter text into '{GetElementLabel(element)}'");
            AppendLine($"{variable.Name}.Enter(\"{SelectorBuilder.EscapeString(text)}\");");
        }

        public override void AssertValue(AutomationElement element, string expected)
        {
            var variable = BuildPathToParent(element);
            AppendLine($"// Assert value of '{GetElementLabel(element)}' equals \"{SelectorBuilder.EscapeString(expected)}\"");
            AppendLine($"NUnit.Framework.Assert.AreEqual(\"{SelectorBuilder.EscapeString(expected)}\", {variable.Name}.AsTextBox()?.Text ?? {variable.Name}.Patterns.Value.Pattern.Value);");
        }

        public override void Wait(int durationMs)
        {
            if (durationMs <= 0)
                AppendLine("FlaUI.Core.Tools.Retry.While(() => false, r => r, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1)); // Wait for input processing");
            else
                AppendLine($"System.Threading.Thread.Sleep({durationMs}); // Wait {durationMs}ms");
        }

        public override void KeyPress(string keyName)
        {
            AppendLine($"// Press key '{keyName}'");
            AppendLine(KeyPressCodeGenerator.BuildCSharpKeyPress(keyName));
        }

        public override string Generate()
        {
            return _builder.ToString();
        }

        protected override Variable BuildPathToParent(AutomationElement element)
        {
            return BuildPathToParentCore(element, new HashSet<AutomationElement>(), _existingVariables, CreateVariableName, AppendRetryCommentIfNeeded);
        }

        protected override void AppendFindStatement(string variableName, string parentVariableName, SelectorInfo selector)
        {
            string findExpression;
            string label = string.IsNullOrEmpty(selector.Name) ? selector.AutomationId ?? selector.ControlType?.ToString() ?? "element" : selector.Name;

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)
            {
                findExpression = $"{parentVariableName}.FindAllChildren(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";
                AppendLine($"// Find '{label}'");
                AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");
                AppendLine($"    () => {findExpression},");
                AppendLine($"    e => e == null,");
                AppendLine($"    TimeSpan.FromSeconds(5),");
                AppendLine($"    TimeSpan.FromMilliseconds(200)).Result;");
            }
            else
            {
                var condition = SelectorBuilder.BuildCSharpCondition(selector);
                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";
                findExpression = $"{parentVariableName}.{findMethod}(cf => {condition})";
                AppendLine($"// Find '{label}'");
                AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");
                AppendLine($"    () => {findExpression},");
                AppendLine($"    e => e == null,");
                AppendLine($"    TimeSpan.FromSeconds(5),");
                AppendLine($"    TimeSpan.FromMilliseconds(200)).Result;");
            }
        }

        protected override void AppendNullCheck(string variableName, SelectorInfo selector)
        {
            AppendLine($"if ({variableName} == null) throw new InvalidOperationException(\"Could not find element '{selector.Description}'\");");
        }

        protected override string FormatMenuWaitHelper()
        {
            return "void WaitForMenuAnimation() { System.Threading.Thread.Sleep(500); }";
        }

        protected override string FormatMenuWaitStatement()
        {
            return "WaitForMenuAnimation();";
        }

        protected override void AppendLine(string line)
        {
            _builder.AppendLine(line);
        }

        private string CreateVariableName(AutomationElement element)
        {
            var baseName = CSharpCodeHelper.GetVariableName(element);
            return EnsureUniqueVariableName(baseName);
        }

        private string EnsureUniqueVariableName(string baseName)
        {
            if (!_existingVariables.Any(v => string.Equals(v.Name, baseName, StringComparison.OrdinalIgnoreCase)))
                return baseName;

            for (var index = 2; ; index++)
            {
                var candidate = baseName + "_" + index;
                if (!_existingVariables.Any(v => string.Equals(v.Name, candidate, StringComparison.OrdinalIgnoreCase)))
                    return candidate;
            }
        }

        private void AppendRetryCommentIfNeeded()
        {
            if (_retryCommentAdded)
                return;

            _retryCommentAdded = true;
            AppendLine(RetryFindComment);
        }

        public override void RecordAction(string automationId, string name, string controlType, string actionType, string comment)
        {
        }
    }
}