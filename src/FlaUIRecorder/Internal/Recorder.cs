using System;
using System.Collections.Generic;
using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.Internal.Worker;

namespace FlaUIRecorder.Internal
{
    public class Recorder : IDisposable
    {
        private readonly AutomationBase _automation;
        private readonly ICodeProvider _codeProvider;
        private readonly List<IRecordWorker> _workers = new List<IRecordWorker>();
        private readonly HoverWorker _hoverWorker;

        public List<RecordedAction> Actions { get; } = new List<RecordedAction>();
        public RecorderState State { get; private set; }

        public event EventHandler<RecordedAction> ActionRecorded;
        public event EventHandler<string> StatusChanged;

        public Recorder(AutomationType automationType, CodeProviderFactory codeProviderFactory, string codeProviderName, Process targetProcess)
        {
            _automation = automationType == AutomationType.UIA2 ? (AutomationBase)new FlaUI.UIA2.UIA2Automation() : new FlaUI.UIA3.UIA3Automation();
            _codeProvider = CreateCodeProvider(codeProviderFactory, codeProviderName, targetProcess);
            State = RecorderState.Paused;

            var hoverCache = new HoverElementCache();
            _hoverWorker = new HoverWorker(_automation, targetProcess.Id, hoverCache);
            _hoverWorker.StatusChanged += Worker_StatusChanged;
            _workers.Add(_hoverWorker);

            var click = new ClickRecognizeWorker(_automation, targetProcess.Id, hoverCache);
            click.ElementClicked += Click_ElementClicked;
            click.ElementRightClicked += Click_ElementRightClicked;
            click.StatusChanged += Worker_StatusChanged;
            _workers.Add(click);

            var keyboard = new KeyboardWorker(_automation, targetProcess.Id, hoverCache);
            keyboard.TextInputCompleted += Keyboard_TextInputCompleted;
            keyboard.KeyPressed += Keyboard_KeyPressed;
            keyboard.StatusChanged += Worker_StatusChanged;
            _workers.Add(keyboard);
        }

        public AutomationElement GetCurrentHoveredElement() => _hoverWorker?.CurrentHoveredElement;

        private ICodeProvider CreateCodeProvider(CodeProviderFactory codeProviderFactory, string codeProviderName, Process targetProcess)
        {
            var args = new CodeProviderArguments
            {
                Automation = _automation,
                TargetProcess = targetProcess
            };

            return codeProviderFactory.CreateProvider(codeProviderName, args);
        }

        public void Dispose()
        {
            _workers.ForEach(w => w.Dispose());
        }

        public void Record()
        {
            State = RecorderState.Recording;
            _workers.ForEach(w => w.Start());
        }

        public void Pause()
        {
            State = RecorderState.Paused;
            _workers.ForEach(w => w.Pause());
        }

        public void AddComment(string comment)
        {
            _codeProvider.AddComment(comment);
            var action = new RecordedAction
            {
                Type = ActionType.Comment,
                Comment = comment,
                Timestamp = DateTime.Now
            };
            Actions.Add(action);
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.Comment.ToString(), comment);
            ActionRecorded?.Invoke(this, action);
        }

        public void InsertWait(int durationMs = 500)
        {
            _codeProvider.Wait(durationMs);
            var action = new RecordedAction
            {
                Type = ActionType.Wait,
                WaitDurationMs = durationMs,
                Timestamp = DateTime.Now
            };
            Actions.Add(action);
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.Wait.ToString(), string.Empty);
            ActionRecorded?.Invoke(this, action);
        }

        public void AddAssertion(AutomationElement element)
        {
            if (element == null)
                return;

            var expected = GetElementValue(element);
            _codeProvider.AssertValue(element, expected);
            var action = CreateActionFromElement(element, ActionType.Assert);
            action.ExpectedValue = expected;
            Actions.Add(action);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.Assert.ToString(), expected);
            ActionRecorded?.Invoke(this, action);
        }

        public string GenerateCode()
        {
            return _codeProvider.Generate();
        }

        private void Click_ElementRightClicked(object sender, AutomationElement e)
        {
            RecordActionFromElement(e, ActionType.RightClick);
            _codeProvider.RightClick(e);
        }

        private void Click_ElementClicked(object sender, AutomationElement e)
        {
            RecordActionFromElement(e, ActionType.Click);
            _codeProvider.Click(e);
        }

        private void Keyboard_TextInputCompleted(object sender, KeyboardActionEventArgs e)
        {
            if (e?.Element == null || string.IsNullOrEmpty(e.Text))
                return;

            var action = CreateActionFromElement(e.Element, ActionType.TextInput);
            action.TextValue = e.Text;
            Actions.Add(action);
            _codeProvider.TextInput(e.Element, e.Text);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.TextInput.ToString(), e.Text);
            ActionRecorded?.Invoke(this, action);
        }

        private void Keyboard_KeyPressed(object sender, KeyboardActionEventArgs e)
        {
            if (string.IsNullOrEmpty(e?.KeyName))
                return;

            var action = new RecordedAction
            {
                Type = ActionType.KeyPress,
                KeyName = e.KeyName,
                Timestamp = DateTime.Now
            };
            Actions.Add(action);
            _codeProvider.KeyPress(e.KeyName);
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.KeyPress.ToString(), e.KeyName);
            ActionRecorded?.Invoke(this, action);
        }

        private void RecordActionFromElement(AutomationElement e, ActionType type)
        {
            var action = CreateActionFromElement(e, type);
            Actions.Add(action);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, type.ToString(), string.Empty);
            ActionRecorded?.Invoke(this, action);
        }

        private static RecordedAction CreateActionFromElement(AutomationElement e, ActionType type)
        {
            string automationId = string.Empty;
            string name = string.Empty;
            string controlType = string.Empty;

            if (e.Properties.AutomationId.TryGetValue(out var aid) && !string.IsNullOrEmpty(aid))
                automationId = aid;
            if (e.Properties.Name.TryGetValue(out var n) && !string.IsNullOrEmpty(n))
                name = n;
            if (e.Properties.ControlType.TryGetValue(out var ct))
                controlType = ct.ToString();

            return new RecordedAction
            {
                Type = type,
                AutomationId = automationId,
                Name = name,
                ControlType = controlType,
                Timestamp = DateTime.Now
            };
        }

        private static string GetElementValue(AutomationElement element)
        {
            try
            {
                if (element.Patterns.Value.IsSupported)
                    return element.Patterns.Value.Pattern.Value ?? string.Empty;

                return element.AsTextBox()?.Text ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void Worker_StatusChanged(object sender, string status)
        {
            StatusChanged?.Invoke(this, status);
        }
    }
}
