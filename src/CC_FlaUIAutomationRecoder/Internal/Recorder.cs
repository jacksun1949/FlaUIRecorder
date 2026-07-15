using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using CC_FlaUIAutomationRecoder.CodeProvider.Common;
using CC_FlaUIAutomationRecoder.Internal.Worker;
using System.Text.Json;

namespace CC_FlaUIAutomationRecoder.Internal
{
    public class Recorder : IDisposable
    {
        private const int ActionPersistInterval = 50;

        private readonly AutomationBase _automation;
        private readonly ICodeProvider _codeProvider;
        private readonly List<IRecordWorker> _workers = new List<IRecordWorker>();
        private readonly HoverWorker _hoverWorker;
        private readonly ClickRecognizeWorker _clickWorker;
        private readonly KeyboardWorker _keyboardWorker;
        private readonly int _targetProcessId;
        private readonly object _actionsLock = new object();
        private AutomationElement _selectedElementOverride;
        private int _actionsSinceLastPersist;

        public List<RecordedAction> Actions { get; } = new List<RecordedAction>();
        public RecorderState State { get; private set; }
        public AutomationBase Automation => _automation;
        public int TargetProcessId => _targetProcessId;

        public event EventHandler<RecordedAction> ActionRecorded;
        public event EventHandler<string> StatusChanged;

        public Recorder(AutomationType automationType, CodeProviderFactory codeProviderFactory, string codeProviderName, Process targetProcess)
        {
            _automation = automationType == AutomationType.UIA2 ? (AutomationBase)new FlaUI.UIA2.UIA2Automation() : new FlaUI.UIA3.UIA3Automation();
            _codeProvider = CreateCodeProvider(codeProviderFactory, codeProviderName, targetProcess);
            _targetProcessId = targetProcess.Id;
            State = RecorderState.Paused;

            var hoverCache = new HoverElementCache();
            _hoverWorker = new HoverWorker(_automation, _targetProcessId, hoverCache);
            _hoverWorker.StatusChanged += Worker_StatusChanged;
            _hoverWorker.HoverStayCompleted += Hover_HoverStayCompleted;
            _workers.Add(_hoverWorker);

            _clickWorker = new ClickRecognizeWorker(_automation, _targetProcessId, hoverCache);
            _clickWorker.ElementClicked += Click_ElementClicked;
            _clickWorker.ElementRightClicked += Click_ElementRightClicked;
            _clickWorker.ElementDoubleClicked += Click_ElementDoubleClicked;
            _clickWorker.DragCompleted += Click_DragCompleted;
            _clickWorker.Scrolled += Click_Scrolled;
            _clickWorker.StatusChanged += Worker_StatusChanged;
            _workers.Add(_clickWorker);

            _keyboardWorker = new KeyboardWorker(_automation, _targetProcessId, hoverCache);
            _keyboardWorker.TextInputCompleted += Keyboard_TextInputCompleted;
            _keyboardWorker.KeyPressed += Keyboard_KeyPressed;
            _keyboardWorker.StatusChanged += Worker_StatusChanged;
            _workers.Add(_keyboardWorker);
        }

        public AutomationElement GetCurrentHoveredElement() => _selectedElementOverride ?? _hoverWorker?.CurrentHoveredElement;

        public void SetSelectedElement(AutomationElement element)
        {
            _selectedElementOverride = element;
            StatusChanged?.Invoke(this, "Element manually selected for next action");
        }

        public void RestartHooks()
        {
            if (State != RecorderState.Recording)
                return;

            Pause();
            Record();
            StatusChanged?.Invoke(this, "Hooks restarted");
        }

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
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.Comment.ToString(), comment);
            NotifyActionRecorded(action);
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
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.Wait.ToString(), string.Empty);
            NotifyActionRecorded(action);
        }

        public void AddAssertion(AutomationElement element)
        {
            if (element == null)
                return;

            var expected = GetElementValue(element);
            _codeProvider.AssertValue(element, expected);
            var action = CreateActionFromElement(element, ActionType.Assert);
            action.ExpectedValue = expected;
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.Assert.ToString(), expected);
            NotifyActionRecorded(action);
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
            var element = ApplyElementOverride(e);
            RecordActionFromElement(element, ActionType.Click);
            _codeProvider.Click(element);
            _selectedElementOverride = null;
        }

        private void Click_ElementDoubleClicked(object sender, AutomationElement e)
        {
            var element = ApplyElementOverride(e);
            RecordActionFromElement(element, ActionType.DoubleClick);
            _codeProvider.DoubleClick(element);
            _selectedElementOverride = null;
        }

        private void Click_DragCompleted(object sender, DragActionEventArgs e)
        {
            if (e?.FromElement == null || e.ToElement == null)
                return;

            var fromAction = CreateActionFromElement(e.FromElement, ActionType.Drag);
            FillTargetElement(fromAction, e.ToElement);
            lock (_actionsLock) { Actions.Add(fromAction); }
            _codeProvider.Drag(e.FromElement, e.ToElement);
            _codeProvider.RecordAction(fromAction.AutomationId, fromAction.Name, fromAction.ControlType, ActionType.Drag.ToString(), fromAction.TargetAutomationId);
            NotifyActionRecorded(fromAction);
        }

        private void Click_Scrolled(object sender, ScrollActionEventArgs e)
        {
            if (e?.Element == null)
                return;

            var action = CreateActionFromElement(e.Element, ActionType.Scroll);
            action.ScrollDelta = e.Delta;
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.Scroll(e.Element, e.Delta);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.Scroll.ToString(), e.Delta.ToString());
            NotifyActionRecorded(action);
        }

        private void Hover_HoverStayCompleted(object sender, HoverStayEventArgs e)
        {
            if (e?.Element == null)
                return;

            var action = CreateActionFromElement(e.Element, ActionType.HoverStay);
            action.HoverDurationMs = e.DurationMs;
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.HoverStay(e.Element, e.DurationMs);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.HoverStay.ToString(), e.DurationMs.ToString());
            NotifyActionRecorded(action);
        }

        private AutomationElement ApplyElementOverride(AutomationElement defaultElement)
        {
            if (_selectedElementOverride != null)
            {
                var selected = _selectedElementOverride;
                _selectedElementOverride = null;
                return selected;
            }
            return defaultElement;
        }

        private void Keyboard_TextInputCompleted(object sender, KeyboardActionEventArgs e)
        {
            if (e?.Element == null || string.IsNullOrEmpty(e.Text))
                return;

            var action = CreateActionFromElement(e.Element, ActionType.TextInput);
            action.TextValue = e.Text;
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.TextInput(e.Element, e.Text);
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, ActionType.TextInput.ToString(), e.Text);
            NotifyActionRecorded(action);
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
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.KeyPress(e.KeyName);
            _codeProvider.RecordAction(string.Empty, string.Empty, string.Empty, ActionType.KeyPress.ToString(), e.KeyName);
            NotifyActionRecorded(action);
        }

        private void RecordActionFromElement(AutomationElement e, ActionType type)
        {
            var action = CreateActionFromElement(e, type);
            lock (_actionsLock) { Actions.Add(action); }
            _codeProvider.RecordAction(action.AutomationId, action.Name, action.ControlType, type.ToString(), string.Empty);
            NotifyActionRecorded(action);
        }

        private void NotifyActionRecorded(RecordedAction action)
        {
            ActionRecorded?.Invoke(this, action);
            PersistActionsIfNeeded();
        }

        private void PersistActionsIfNeeded()
        {
            _actionsSinceLastPersist++;
            if (_actionsSinceLastPersist < ActionPersistInterval)
                return;

            _actionsSinceLastPersist = 0;
            try
            {
                var path = Path.Combine(Path.GetTempPath(), "CC_FlaUIAutomationRecoder_actions_temp.json");
                string json;
                lock (_actionsLock)
                {
                    json = JsonSerializer.Serialize(Actions);
                }
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "Recorder.PersistActions");
            }
        }

        private static RecordedAction CreateActionFromElement(AutomationElement e, ActionType type)
        {
            string automationId = string.Empty;
            string name = string.Empty;
            string controlType = string.Empty;

            try
            {
                if (e.Properties.AutomationId.TryGetValue(out var aid) && !string.IsNullOrEmpty(aid))
                    automationId = aid;
            }
            catch (Exception ex) { RecorderErrorLog.RecordError(ex, "Recorder.CreateActionFromElement.AutomationId"); }

            try
            {
                if (e.Properties.Name.TryGetValue(out var n) && !string.IsNullOrEmpty(n))
                    name = n;
            }
            catch (Exception ex) { RecorderErrorLog.RecordError(ex, "Recorder.CreateActionFromElement.Name"); }

            try
            {
                if (e.Properties.ControlType.TryGetValue(out var ct))
                    controlType = ct.ToString();
            }
            catch (Exception ex) { RecorderErrorLog.RecordError(ex, "Recorder.CreateActionFromElement.ControlType"); }

            return new RecordedAction
            {
                Type = type,
                AutomationId = automationId,
                Name = name,
                ControlType = controlType,
                Timestamp = DateTime.Now
            };
        }

        private static void FillTargetElement(RecordedAction action, AutomationElement target)
        {
            try
            {
                if (target.Properties.AutomationId.TryGetValue(out var aid) && !string.IsNullOrEmpty(aid))
                    action.TargetAutomationId = aid;
                if (target.Properties.Name.TryGetValue(out var n) && !string.IsNullOrEmpty(n))
                    action.TargetName = n;
                if (target.Properties.ControlType.TryGetValue(out var ct))
                    action.TargetControlType = ct.ToString();
            }
            catch (Exception ex) { RecorderErrorLog.RecordError(ex, "Recorder.FillTargetElement"); }
        }

        private static string GetElementValue(AutomationElement element)
        {
            try
            {
                if (element.Patterns.Value.IsSupported)
                    return element.Patterns.Value.Pattern.Value ?? string.Empty;

                return element.AsTextBox()?.Text ?? string.Empty;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "Recorder.GetElementValue");
                return string.Empty;
            }
        }

        private void Worker_StatusChanged(object sender, string status)
        {
            StatusChanged?.Invoke(this, status);
        }
    }
}
