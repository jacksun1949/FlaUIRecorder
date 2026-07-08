using FlaUI.Core.Definitions;

namespace FlaUIRecorder.CodeProvider.Common
{
    public enum SelectorFindMethod
    {
        FindFirstDescendant,
        FindFirstChild,
        FindAllChildren
    }

    public class SelectorInfo
    {
        public string AutomationId { get; set; }
        public string Name { get; set; }
        public ControlType? ControlType { get; set; }
        public bool RequireEnabled { get; set; }
        public SelectorFindMethod FindMethod { get; set; }
        public int ChildIndex { get; set; }

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(AutomationId))
                    return AutomationId;
                if (!string.IsNullOrEmpty(Name))
                    return Name;
                if (ControlType.HasValue)
                    return ControlType.Value.ToString();
                return "element";
            }
        }
    }
}
