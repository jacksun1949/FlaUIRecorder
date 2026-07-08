using FlaUIRecorder.CodeProvider.Common;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using FlaUI.Core.AutomationElements.Infrastructure;

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

            AppendLine($"if ({variable.Name} == null) throw new InvalidOperationException(\"Could not find element '{GetElementLabel(element)}'\");");

            AppendLine($"{variable.Name}.Click();");

            AppendMenuWaitIfNeeded(element);

        }



        public override void RightClick(AutomationElement element)

        {

            var variable = BuildPathToParent(element);

            AppendLine($"if ({variable.Name} == null) throw new InvalidOperationException(\"Could not find element '{GetElementLabel(element)}'\");");

            AppendLine($"{variable.Name}.RightClick();");

            AppendMenuWaitIfNeeded(element);

        }



        public override void TextInput(AutomationElement element, string text)

        {

            var variable = BuildPathToParent(element);

            AppendLine($"if ({variable.Name} == null) throw new InvalidOperationException(\"Could not find element '{GetElementLabel(element)}'\");");

            AppendLine($"{variable.Name}.Enter(\"{SelectorBuilder.EscapeString(text)}\");");

        }



        public override void AssertValue(AutomationElement element, string expected)

        {

            var variable = BuildPathToParent(element);

            AppendLine($"if ({variable.Name} == null) throw new InvalidOperationException(\"Could not find element '{GetElementLabel(element)}'\");");

            AppendLine($"NUnit.Framework.Assert.AreEqual(\"{SelectorBuilder.EscapeString(expected)}\", {variable.Name}.AsTextBox()?.Text ?? {variable.Name}.Patterns.Value.Pattern.Value);");

        }



        public override void Wait(int durationMs)

        {

            if (durationMs <= 0)

                AppendLine("FlaUI.Core.Tools.Retry.While(() => false, TimeSpan.FromMilliseconds(1)); // Wait for input processing");

            else

                AppendLine($"System.Threading.Thread.Sleep({durationMs});");

        }



        public override void KeyPress(string keyName)

        {

            AppendLine($"FlaUI.Core.Input.Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.{keyName});");

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

            if (selector.FindMethod == SelectorFindMethod.FindAllChildren && selector.ControlType.HasValue)

            {

                findExpression = $"{parentVariableName}.FindAllChildren(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.{selector.ControlType.Value}))[{selector.ChildIndex}]";

                AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");

                AppendLine($"    () => {findExpression},");

                AppendLine($"    e => e == null,");

                AppendLine($"    TimeSpan.FromSeconds(5),");

                AppendLine($"    TimeSpan.FromMilliseconds(200));");

            }

            else

            {

                var condition = SelectorBuilder.BuildCSharpCondition(selector);

                var findMethod = selector.FindMethod == SelectorFindMethod.FindFirstChild ? "FindFirstChild" : "FindFirstDescendant";

                findExpression = $"{parentVariableName}.{findMethod}(e => {condition})";

                AppendLine($"var {variableName} = FlaUI.Core.Tools.Retry.While(");

                AppendLine($"    () => {findExpression},");

                AppendLine($"    e => e == null,");

                AppendLine($"    TimeSpan.FromSeconds(5),");

                AppendLine($"    TimeSpan.FromMilliseconds(200));");

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

            var parent = GetParentOrMainWindow(element);

            var elementName = CSharpCodeHelper.GetVariableName(element, parent);

            string suffix;



            if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))

            {

                suffix = SanitizeIdentifier(automationId);

                if (string.IsNullOrEmpty(suffix))

                    suffix = GetVariableIdentifier(elementName);

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

            AppendLine(RetryFindComment);

        }



        private string GetVariableIdentifier(string variableName)

        {

            var amount = _existingVariables.Count(v => v.Name.StartsWith(variableName) && v.Name.Length > variableName.Length && Char.IsDigit(v.Name[variableName.Length]));

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

    }

}


