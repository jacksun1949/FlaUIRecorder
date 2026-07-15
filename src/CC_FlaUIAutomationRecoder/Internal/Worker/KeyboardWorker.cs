using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC_FlaUIAutomationRecoder.Internal.Worker
{
    public class KeyboardWorker : IRecordWorker
    {
        private static readonly HashSet<string> SpecialKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "RETURN", "ENTER", "ESCAPE", "TAB", "BACK", "BACKSPACE", "DELETE", "SPACE",
            "HOME", "END", "PRIOR", "PAGEUP", "NEXT", "PAGEDOWN",
            "LEFT", "RIGHT", "UP", "DOWN",
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12"
        };

        private static readonly HashSet<string> ModifierKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LCONTROL", "RCONTROL", "CONTROL", "LCTRL", "RCTRL", "CTRL",
            "LSHIFT", "RSHIFT", "SHIFT",
            "LALT", "RALT", "ALT", "LMENU", "RMENU"
        };

        private static readonly ControlType[] TextInputControlTypes =
        {
            ControlType.Edit,
            ControlType.Document
        };

        private readonly AutomationBase _automation;
        private readonly HoverElementCache _hoverCache;
        private readonly int _targetProcessId;
        private readonly StringBuilder _textBuffer = new StringBuilder();
        private readonly HashSet<string> _activeModifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HookHealthMonitor _healthMonitor;
        private readonly LowLevelKeyboardHook _keyboardHook;
        private AutomationElement _textTarget;

        public event EventHandler<KeyboardActionEventArgs> TextInputCompleted;
        public event EventHandler<KeyboardActionEventArgs> KeyPressed;
        public event EventHandler<string> StatusChanged;

        public KeyboardWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)
        {
            _automation = automation;
            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));
            _targetProcessId = targetProcessId;
            _healthMonitor = new HookHealthMonitor("Keyboard");
            _healthMonitor.StatusChanged += (s, msg) => StatusChanged?.Invoke(this, msg);
            _keyboardHook = new LowLevelKeyboardHook();
            _keyboardHook.KeyEvent += KeyboardHook_OnKeyEvent;
        }

        public void Dispose()
        {
            Pause();
            _healthMonitor.Dispose();
            _keyboardHook.Dispose();
        }

        public void Start()
        {
            try
            {
                _keyboardHook.Start();
                _healthMonitor.Start();
                StatusChanged?.Invoke(this, "Keyboard hook started");
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "LowLevelKeyboardHook.Start");
                StatusChanged?.Invoke(this, "Keyboard hook failed to start");
            }
        }

        public void Pause()
        {
            FlushTextBufferFromElement();
            _activeModifiers.Clear();
            try
            {
                _keyboardHook.Stop();
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "LowLevelKeyboardHook.Stop");
            }
            _healthMonitor.Stop();
        }

        private void KeyboardHook_OnKeyEvent(KeyEventData e)
        {
            try
            {
                _healthMonitor.RecordEvent();

                if (e?.KeyName == null)
                    return;

                var keyName = e.KeyName.ToUpperInvariant();
                if (string.IsNullOrEmpty(keyName))
                    return;

                if (e.EventType == KeyEventType.Down)
                {
                    if (IsModifierKey(keyName))
                    {
                        _activeModifiers.Add(NormalizeModifier(keyName));
                        return;
                    }

                    if (SpecialKeys.Contains(keyName))
                    {
                        FlushTextBufferFromElement();
                        EmitKeyPress(MapVirtualKey(keyName));
                        return;
                    }

                    if (_activeModifiers.Count > 0)
                    {
                        FlushTextBufferFromElement();
                        EmitKeyPress(BuildCompoundKey(keyName));
                        return;
                    }

                    if (keyName.Length == 1)
                    {
                        var focused = GetFocusedElement();
                        if (!IsTextInputElement(focused))
                            return;

                        if (_textTarget == null || !ReferenceEquals(_textTarget, focused))
                        {
                            FlushTextBufferFromElement();
                            _textTarget = focused;
                        }

                        _textBuffer.Append(keyName);
                    }
                }
                else if (e.EventType == KeyEventType.Up)
                {
                    if (IsModifierKey(keyName))
                        _activeModifiers.Remove(NormalizeModifier(keyName));
                }
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWorker");
            }
        }

        private void EmitKeyPress(string keyName)
        {
            KeyPressed?.Invoke(this, new KeyboardActionEventArgs { KeyName = keyName });
        }

        private string BuildCompoundKey(string keyName)
        {
            var parts = new List<string>();
            if (_activeModifiers.Contains("Ctrl")) parts.Add("Ctrl");
            if (_activeModifiers.Contains("Shift")) parts.Add("Shift");
            if (_activeModifiers.Contains("Alt")) parts.Add("Alt");
            parts.Add(keyName.Length == 1 ? keyName.ToUpperInvariant() : MapVirtualKey(keyName));
            return string.Join("+", parts);
        }

        private static bool IsModifierKey(string keyName)
        {
            return ModifierKeys.Contains(keyName);
        }

        private static string NormalizeModifier(string keyName)
        {
            if (keyName.Contains("SHIFT")) return "Shift";
            if (keyName.Contains("ALT") || keyName.Contains("MENU")) return "Alt";
            return "Ctrl";
        }

        private void FlushTextBufferFromElement()
        {
            if (_textTarget != null)
            {
                var fullText = TryReadElementText(_textTarget);
                if (!string.IsNullOrEmpty(fullText))
                {
                    TextInputCompleted?.Invoke(this, new KeyboardActionEventArgs
                    {
                        Element = _textTarget,
                        Text = fullText
                    });
                }
                else if (_textBuffer.Length > 0)
                {
                    TextInputCompleted?.Invoke(this, new KeyboardActionEventArgs
                    {
                        Element = _textTarget,
                        Text = _textBuffer.ToString()
                    });
                }
            }

            _textBuffer.Clear();
            _textTarget = null;
        }

        private static string TryReadElementText(AutomationElement element)
        {
            if (element == null)
                return null;

            try
            {
                if (element.Patterns.Value.IsSupported)
                {
                    var value = element.Patterns.Value.Pattern.Value;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                if (element.Patterns.Text.IsSupported)
                {
                    var text = element.Patterns.Text.Pattern.DocumentRange.GetText(-1);
                    if (!string.IsNullOrEmpty(text))
                        return text;
                }

                var textBox = element.AsTextBox();
                if (textBox != null && !string.IsNullOrEmpty(textBox.Text))
                    return textBox.Text;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWorker.TryReadElementText");
            }

            return null;
        }

        private AutomationElement GetFocusedElement()
        {
            try
            {
                var focused = _automation.FocusedElement();
                if (IsTargetElement(focused))
                    return focused;

                var mousePos = Mouse.Position;
                if (_hoverCache.TryGetNear((int)mousePos.X, (int)mousePos.Y, out var cached) && IsTargetElement(cached))
                    return cached;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWorker.GetFocusedElement");
            }

            return null;
        }

        private bool IsTextInputElement(AutomationElement element)
        {
            if (!IsTargetElement(element))
                return false;

            try
            {
                if (!element.Properties.ControlType.TryGetValue(out var controlType))
                    return false;

                foreach (var textType in TextInputControlTypes)
                {
                    if (controlType == textType)
                        return true;
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return false;
        }

        private const int IsTargetTimeoutMs = 2000;

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;

            // UIA ProcessId access is a synchronous COM call that can block indefinitely
            // if the target process is frozen/hung. Use a timeout to prevent deadlocks.
            var processId = 0;
            Exception capturedException = null;

            var task = System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    processId = element.Properties.ProcessId;
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            });

            if (!task.Wait(IsTargetTimeoutMs))
            {
                RecorderErrorLog.RecordError(
                    new TimeoutException($"KeyboardWorker.IsTargetElement timed out after {IsTargetTimeoutMs}ms"),
                    "KeyboardWorker.IsTargetElement");
                return false;
            }

            if (capturedException != null)
            {
                RecorderErrorLog.RecordError(capturedException, "KeyboardWorker.IsTargetElement");
                return false;
            }

            return processId == _targetProcessId;
        }

        private static string MapVirtualKey(string keyName)
        {
            switch (keyName)
            {
                case "RETURN":
                    return "Enter";
                case "BACK":
                case "BACKSPACE":
                    return "Back";
                case "PRIOR":
                case "PAGEUP":
                    return "PageUp";
                case "NEXT":
                case "PAGEDOWN":
                    return "PageDown";
                case "ESCAPE":
                    return "Escape";
                case "DELETE":
                    return "Delete";
                case "SPACE":
                    return "Space";
                case "LEFT":
                case "RIGHT":
                case "UP":
                case "DOWN":
                case "HOME":
                case "END":
                case "TAB":
                    return char.ToUpper(keyName[0]) + keyName.Substring(1).ToLowerInvariant();
                default:
                    if (keyName.StartsWith("F") && keyName.Length <= 3)
                        return keyName;
                    return keyName;
            }
        }
    }

    public class KeyboardActionEventArgs : EventArgs
    {
        public AutomationElement Element { get; set; }
        public string Text { get; set; }
        public string KeyName { get; set; }
    }
}
