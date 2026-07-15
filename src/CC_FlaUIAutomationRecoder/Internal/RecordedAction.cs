using System;

namespace CC_FlaUIAutomationRecoder.Internal
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
        KeyPress,
        DoubleClick,
        Drag,
        Scroll,
        HoverStay
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

        // Drag target / secondary element
        public string TargetAutomationId { get; set; }
        public string TargetName { get; set; }
        public string TargetControlType { get; set; }

        // Scroll
        public int ScrollDelta { get; set; }

        // Hover-stay
        public int HoverDurationMs { get; set; }
    }
}
