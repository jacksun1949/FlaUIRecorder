using EventHook;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlaUIRecorder.Internal.Worker
{
    public class KeyboardWorker : IRecordWorker
    {
        private static readonly HashSet<string> SpecialKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "RETURN", "ENTER", "ESCAPE", "TAB"
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
        private AutomationElement _textTarget;

        public event EventHandler<KeyboardActionEventArgs> TextInputCompleted;
        public event EventHandler<KeyboardActionEventArgs> KeyPressed;
        public event EventHandler<string> StatusChanged;

        public KeyboardWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)
        {
            _automation = automation;
            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));
            _targetProcessId = targetProcessId;
            KeyboardWatcher.OnKeyInput += KeyboardWatcher_OnKeyInput;
        }

        public void Dispose()
        {
            Pause();
            KeyboardWatcher.OnKeyInput -= KeyboardWatcher_OnKeyInput;
        }

        public void Start()
        {
            try
            {
                KeyboardWatcher.Start();
                StatusChanged?.Invoke(this, "Keyboard hook started");
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWatcher.Start");
                StatusChanged?.Invoke(this, "Keyboard hook failed to start");
            }
        }

        public void Pause()
        {
            FlushTextBuffer();
            try
            {
                KeyboardWatcher.Stop();
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWatcher.Stop");
            }
        }

        private void KeyboardWatcher_OnKeyInput(object sender, KeyInputEventArgs e)
        {
            try
            {
                if (e?.KeyData == null || e.KeyData.EventType != KeyEvent.down)
                    return;

                var keyName = e.KeyData.Keyname?.ToUpperInvariant() ?? string.Empty;
                if (string.IsNullOrEmpty(keyName))
                    return;

                if (SpecialKeys.Contains(keyName))
                {
                    FlushTextBuffer();
                    KeyPressed?.Invoke(this, new KeyboardActionEventArgs { KeyName = MapVirtualKey(keyName) });
                    return;
                }

                if (keyName.Length == 1)
                {
                    var focused = GetFocusedElement();
                    if (!IsTextInputElement(focused))
                        return;

                    if (_textTarget == null || !ReferenceEquals(_textTarget, focused))
                    {
                        FlushTextBuffer();
                        _textTarget = focused;
                    }

                    _textBuffer.Append(keyName);
                }
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "KeyboardWorker");
            }
        }

        private void FlushTextBuffer()
        {
            if (_textTarget == null || _textBuffer.Length == 0)
            {
                _textBuffer.Clear();
                _textTarget = null;
                return;
            }

            TextInputCompleted?.Invoke(this, new KeyboardActionEventArgs
            {
                Element = _textTarget,
                Text = _textBuffer.ToString()
            });

            _textBuffer.Clear();
            _textTarget = null;
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
            if (!IsTargetElement(element) || !element.Properties.ControlType.TryGetValue(out var controlType))
                return false;

            foreach (var textType in TextInputControlTypes)
            {
                if (controlType == textType)
                    return true;
            }

            return false;
        }

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;

            try
            {
                return element.Properties.ProcessId == _targetProcessId;
            }
            catch
            {
                return false;
            }
        }

        private static string MapVirtualKey(string keyName)
        {
            if (keyName == "RETURN")
                return "ENTER";

            return keyName;
        }
    }

    public class KeyboardActionEventArgs : EventArgs
    {
        public AutomationElement Element { get; set; }
        public string Text { get; set; }
        public string KeyName { get; set; }
    }
}
