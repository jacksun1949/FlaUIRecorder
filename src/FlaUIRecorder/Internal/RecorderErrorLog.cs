using System;
using System.Collections.Generic;

namespace FlaUIRecorder.Internal
{
    public static class RecorderErrorLog
    {
        private static readonly object SyncRoot = new object();
        private static readonly List<string> ErrorLog = new List<string>();

        public static IReadOnlyList<string> Errors
        {
            get
            {
                lock (SyncRoot)
                {
                    return ErrorLog.ToArray();
                }
            }
        }

        public static int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return ErrorLog.Count;
                }
            }
        }

        public static event EventHandler ErrorRecorded;

        public static void RecordError(Exception ex, string context = null)
        {
            if (ex == null)
                return;

            var message = string.IsNullOrEmpty(context)
                ? $"{DateTime.Now:HH:mm:ss} {ex.GetType().Name}: {ex.Message}"
                : $"{DateTime.Now:HH:mm:ss} [{context}] {ex.GetType().Name}: {ex.Message}";

            lock (SyncRoot)
            {
                ErrorLog.Add(message);
            }

            ErrorRecorded?.Invoke(null, EventArgs.Empty);
        }

        public static void Clear()
        {
            lock (SyncRoot)
            {
                ErrorLog.Clear();
            }
        }
    }
}
