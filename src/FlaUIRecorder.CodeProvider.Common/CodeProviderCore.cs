using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using FlaUI.Core.AutomationElements;
using FlaUI.Core;
using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUIRecorder.CodeProvider.Common.Internals;

namespace FlaUIRecorder.CodeProvider.Common
{
    public abstract class CodeProviderCore : ICodeProvider
    {
        public const string RetryFindComment = "// Tip: For dynamic UIs, retry Find* calls until the element appears or a timeout elapses.";
        public const int MaxPathDepth = 50;

        public AutomationBase Automation { get; private set; }
        public ITreeWalker TreeWalker { get; private set; }
        public AutomationElement RootElement { get; private set; }
        public Process TargetProcess { get; private set; }

        private Window _mainWindow;

        public CodeProviderCore(CodeProviderArguments args)
        {
            Automation = args.Automation;
            TargetProcess = args.TargetProcess;

            RootElement = FindTargetApplicationRoot(args.TargetProcess.Id);

            TreeWalker = Automation.TreeWalkerFactory.GetControlViewWalker();
        }

        protected Window GetMainWindow()
        {
            if (_mainWindow == null)
            {
                IntPtr mainWindowHandle = TargetProcess.MainWindowHandle;

                if (mainWindowHandle == IntPtr.Zero)
                    return null;

                _mainWindow = Automation.FromHandle(mainWindowHandle).AsWindow();
            }

            return _mainWindow;
        }

        protected List<AutomationElement> GetElementPathToRoot(AutomationElement obj)
        {
            var pathToRoot = new List<AutomationElement>();
            var visited = new HashSet<AutomationElement>();
            while (obj != null)
            {
                if (visited.Contains(obj) || pathToRoot.Contains(obj))
                    break;

                try
                {
                    if (obj.Equals(RootElement))
                        break;
                }
                catch
                {
                    // Element disconnected; stop traversal.
                    break;
                }

                visited.Add(obj);
                pathToRoot.Add(obj);
                try
                {
                    obj = TreeWalker.GetParent(obj);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetElementPathToRoot: {ex.Message}");
                }
            }

            return pathToRoot;
        }

        protected AutomationElement GetParentOrMainWindow(AutomationElement element)
        {
            return GetPathParent(element);
        }

        protected AutomationElement GetPathParent(AutomationElement element)
        {
            var mainWindow = GetMainWindow();
            if (element == null)
                return mainWindow;

            if (IsMainWindow(element))
                return null;

            AutomationElement parent = null;
            try
            {
                parent = TreeWalker.GetParent(element);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            parent = SkipShellAncestors(parent);

            if (parent == null)
                return mainWindow;

            if (IsMainWindow(parent))
                return mainWindow;

            if (RootElement != null && mainWindow != null)
            {
                try
                {
                    if (parent.Equals(RootElement))
                        return mainWindow;
                }
                catch
                {
                    // Element may be disconnected during comparison.
                }
            }

            if (mainWindow != null && !BelongsToTargetProcess(parent))
                return mainWindow;

            return parent;
        }

        protected AutomationElement SkipShellAncestors(AutomationElement element)
        {
            var current = element;
            var visited = new HashSet<AutomationElement>();

            while (current != null)
            {
                if (visited.Contains(current))
                    return null;

                visited.Add(current);

                if (!ShellElementFilter.IsShellOrDesktopElement(current, RootElement, Automation))
                    return current;

                try
                {
                    current = TreeWalker.GetParent(current);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        protected bool IsMainWindow(AutomationElement element)
        {
            var mainWindow = GetMainWindow();
            if (mainWindow == null || element == null)
                return false;

            if (ReferenceEquals(element, mainWindow))
                return true;

            try
            {
                if (element.Properties.NativeWindowHandle.TryGetValue(out var elementHandle)
                    && mainWindow.Properties.NativeWindowHandle.TryGetValue(out var mainWindowHandle)
                    && elementHandle != IntPtr.Zero
                    && elementHandle == mainWindowHandle)
                {
                    return true;
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return false;
        }

        protected bool BelongsToTargetProcess(AutomationElement element)
        {
            if (element == null || TargetProcess == null)
                return false;

            try
            {
                if (!element.Properties.ProcessId.TryGetValue(out var processId))
                    return true;

                return processId == TargetProcess.Id;
            }
            catch (System.Runtime.InteropServices.COMException) { return true; }
            catch (InvalidOperationException) { return true; }
        }

        protected int GetControlTypeChildIndex(AutomationElement element)
        {
            try
            {
                var parent = GetParentOrMainWindow(element);
                if (parent == null || !element.Properties.ControlType.TryGetValue(out var controlType))
                    return 0;

                var children = parent.FindAllChildren(c => c.ByControlType(controlType));
                for (var i = 0; i < children.Length; i++)
                {
                    if (ReferenceEquals(children[i], element))
                        return i;

                    if (element.Properties.AutomationId.TryGetValue(out var automationId)
                        && !string.IsNullOrEmpty(automationId)
                        && children[i].Properties.AutomationId.TryGetValue(out var childAutomationId)
                        && childAutomationId == automationId)
                    {
                        return i;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return 0;
        }

        protected static string SanitizeIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var sanitized = Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);
            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return sanitized;
        }

        protected static string GetElementLabel(AutomationElement element)
        {
            if (element == null)
                return "(null)";

            try
            {
                string name = null;
                element.Properties.Name.TryGetValue(out name);
                if (!string.IsNullOrEmpty(name))
                    return name;

                string automationId = null;
                element.Properties.AutomationId.TryGetValue(out automationId);
                if (!string.IsNullOrEmpty(automationId))
                    return automationId;

                if (element.Properties.ControlType.TryGetValue(out var controlType))
                    return controlType.ToString();
            }
            catch { }

            return "element";
        }

        protected SelectorInfo BuildSelector(AutomationElement element, bool requireEnabled = true)
        {
            var selector = SelectorBuilder.Build(element, requireEnabled);
            if (selector.FindMethod == SelectorFindMethod.FindAllChildren)
                selector.ChildIndex = GetControlTypeChildIndex(element);
            return selector;
        }

        protected abstract Variable BuildPathToParent(AutomationElement element);

        protected abstract void AppendFindStatement(string variableName, string parentVariableName, SelectorInfo selector);

        protected abstract void AppendNullCheck(string variableName, SelectorInfo selector);

        protected abstract string FormatMenuWaitStatement();

        protected abstract string FormatMenuWaitHelper();

        protected abstract bool MenuWaitHelperAdded { get; set; }
        protected bool SafeClickHelperAdded { get; set; }

        protected void AppendSafeClickHelperIfNeeded()
        {
            if (SafeClickHelperAdded)
                return;

            SafeClickHelperAdded = true;
            AppendLine(SafeClickCodeGenerator.CSharpLocalHelperMethod);
        }

        protected Variable BuildPathToParentCore(
            AutomationElement element,
            HashSet<AutomationElement> visited,
            VariableList existingVariables,
            Func<AutomationElement, string> createVariableName,
            Action appendRetryCommentIfNeeded)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (visited.Contains(element))
                throw new InvalidOperationException("Circular element reference detected while building automation path.");

            if (visited.Count >= MaxPathDepth)
                throw new InvalidOperationException($"Element path exceeded maximum depth of {MaxPathDepth}.");

            visited.Add(element);

            if (IsMainWindow(element))
                return existingVariables["window"];

            if (ShellElementFilter.IsShellOrDesktopElement(element, RootElement, Automation))
                return existingVariables["window"];

            var parent = GetPathParent(element);
            if (parent == null)
                return existingVariables["window"];

            var parentVariable = existingVariables.Find(parent);

            if (parentVariable == null)
                parentVariable = BuildPathToParentCore(parent, visited, existingVariables, createVariableName, appendRetryCommentIfNeeded);

            var selector = BuildSelector(element);
            var existingVariable = existingVariables.FindByParentAndSelector(parentVariable.Name, selector);
            if (existingVariable != null)
                return existingVariable;

            var variableName = createVariableName(element);

            appendRetryCommentIfNeeded();
            AppendFindStatement(variableName, parentVariable.Name, selector);
            AppendNullCheck(variableName, selector);

            return existingVariables.Add(variableName, element, parentVariable.Name, selector);
        }

        protected void AppendMenuWaitIfNeeded(AutomationElement element)
        {
            if (!element.IsMenuItem())
                return;

            if (!MenuWaitHelperAdded)
            {
                AppendLine(FormatMenuWaitHelper());
                MenuWaitHelperAdded = true;
            }

            AppendLine(FormatMenuWaitStatement());
        }

        protected abstract void AppendLine(string line);

        private AutomationElement FindTargetApplicationRoot(int targetProcessId)
        {
            var element = Automation.GetDesktop();
            var root = element.FindFirstChild(c => c.ByProcessId(targetProcessId));

            return root;
        }

        public abstract void AddComment(string comment);
        public abstract void Click(AutomationElement element);
        public abstract void RightClick(AutomationElement element);
        public abstract void TextInput(AutomationElement element, string text);
        public abstract void AssertValue(AutomationElement element, string expected);
        public abstract void Wait(int durationMs);
        public abstract void KeyPress(string keyName);
        public abstract void DoubleClick(AutomationElement element);
        public abstract void Drag(AutomationElement fromElement, AutomationElement toElement);
        public abstract void Scroll(AutomationElement element, int delta);
        public abstract void HoverStay(AutomationElement element, int durationMs);
        public abstract string Generate();

        public virtual void RecordAction(string automationId, string name, string controlType, string actionType, string comment)
        {
        }
    }
}

