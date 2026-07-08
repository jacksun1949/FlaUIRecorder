using System;

namespace FlaUIRecorder.Internal
{
    [Serializable]
    public enum ActionType
    {
        Click,
        RightClick,
        Comment,
        Wait,
        TextInput,
        Assert,
        KeyPress
    }

    [Serializable]
    public class RecordedAction
    {
        public ActionType Type { get; set; }
        public string AutomationId { get; set; }
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string ParentPath { get; set; }
        public string Comment { get; set; }
        public string TextValue { get; set; }
        public string ExpectedValue { get; set; }
        public int WaitDurationMs { get; set; }
        public string KeyName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
