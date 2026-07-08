using FlaUI.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlaUIRecorder.Internal
{
    [Serializable]
    [DebuggerDisplay("{Code}")]
    public class RecordSession
    {
        public DateTime StartTime { get; set; }
        public string Code { get; set; }
        public List<RecordedAction> Actions { get; set; } = new List<RecordedAction>();

        public override string ToString()
        {
            var actionCount = Actions?.Count ?? 0;
            var lastAction = actionCount > 0 ? Actions[actionCount - 1].Type.ToString() : "none";
            return $"{StartTime:g} ({actionCount} actions, last: {lastAction})";
        }

        public override int GetHashCode()
        {
            return StartTime.GetHashCode();
        }
    }
}
