using FlaUI.Core.AutomationElements.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaUIRecorder.CodeProvider.Common
{
    public interface ICodeProvider
    {
        void AddComment(string comment);
        void Click(AutomationElement element);
        void RightClick(AutomationElement element);
        void TextInput(AutomationElement element, string text);
        void AssertValue(AutomationElement element, string expected);
        void Wait(int durationMs);
        void KeyPress(string keyName);
        void DoubleClick(AutomationElement element);
        void Drag(AutomationElement fromElement, AutomationElement toElement);
        void Scroll(AutomationElement element, int delta);
        void HoverStay(AutomationElement element, int durationMs);
        string Generate();
        void RecordAction(string automationId, string name, string controlType, string actionType, string comment);
    }
}

