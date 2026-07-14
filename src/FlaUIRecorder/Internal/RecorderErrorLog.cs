using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlaUIRecorder.Internal
{
    /// <summary>
    /// Thread-safe in-memory error log with persistent file backup.
    /// Stores full exception chain (message, stack trace, inner exceptions, data).
    /// </summary>
    public static class RecorderErrorLog
    {
        private const int MaxEntries = 2000;
        private static readonly object SyncRoot = new object();
        private static readonly List<ErrorEntry> ErrorLog = new List<ErrorEntry>();
        private static string _logFilePath;
        private static bool _fileLogInitialized;

        public static IReadOnlyList<string> Errors
        {
            get
            {
                lock (SyncRoot)
                {
                    var snapshot = new string[ErrorLog.Count];
                    for (int i = 0; i < ErrorLog.Count; i++)
                        snapshot[i] = ErrorLog[i].Summary;
                    return snapshot;
                }
            }
        }

        public static int Count
        {
            get
            {
                lock (SyncRoot) { return ErrorLog.Count; }
            }
        }

        public static string LogFilePath
        {
            get
            {
                EnsureFileLog();
                return _logFilePath;
            }
        }

        public static event EventHandler ErrorRecorded;

        /// <summary>
        /// Record an exception with full diagnostic context.
        /// </summary>
        public static void RecordError(Exception ex, string context = null)
        {
            if (ex == null)
                return;

            var entry = new ErrorEntry
            {
                Timestamp = DateTime.Now,
                Context = context ?? string.Empty,
                FullDetail = FormatExceptionChain(ex)
            };

            lock (SyncRoot)
            {
                ErrorLog.Add(entry);
                while (ErrorLog.Count > MaxEntries)
                    ErrorLog.RemoveAt(0);
            }

            // Fire event for UI updates
            ErrorRecorded?.Invoke(null, EventArgs.Empty);

            // Persist to file (fire-and-forget, don't let file IO block or throw)
            AppendToFile(entry);
        }

        /// <summary>
        /// Format the full exception chain: type, message, stack trace, inner exceptions, data.
        /// </summary>
        private static string FormatExceptionChain(Exception ex)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();
            FormatException(ex, sb, 0);
            return sb.ToString();
        }

        private static void FormatException(Exception ex, StringBuilder sb, int depth)
        {
            var indent = new string(' ', depth * 2);
            sb.AppendLine($"{indent}[{ex.GetType().FullName}] {ex.Message}");

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                foreach (var line in ex.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    sb.AppendLine($"{indent}  at {line.Trim()}");
            }

            // Include exception Data dictionary (custom diagnostic info)
            if (ex.Data != null && ex.Data.Count > 0)
            {
                sb.AppendLine($"{indent}  Data:");
                foreach (System.Collections.DictionaryEntry kv in ex.Data)
                    sb.AppendLine($"{indent}    {kv.Key}: {kv.Value}");
            }

            // Recurse into aggregated exceptions
            if (ex is AggregateException agg)
            {
                foreach (var inner in agg.InnerExceptions)
                {
                    sb.AppendLine($"{indent}  ---> (aggregated)");
                    FormatException(inner, sb, depth + 1);
                }
            }
            else if (ex.InnerException != null)
            {
                sb.AppendLine($"{indent}  ---> (inner)");
                FormatException(ex.InnerException, sb, depth + 1);
            }
        }

        private static void EnsureFileLog()
        {
            if (_fileLogInitialized)
                return;

            try
            {
                var dir = Path.Combine(Path.GetTempPath(), "FlaUIRecorder");
                Directory.CreateDirectory(dir);
                _logFilePath = Path.Combine(dir, "error.log");
                _fileLogInitialized = true;
            }
            catch
            {
                // Cannot create log directory; file logging disabled
                _fileLogInitialized = true;
            }
        }

        private static void AppendToFile(ErrorEntry entry)
        {
            try
            {
                EnsureFileLog();
                if (string.IsNullOrEmpty(_logFilePath))
                    return;

                var line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Context}] {entry.FullDetail}";
                // Limit rotation: keep file under ~2 MB
                try
                {
                    var info = new FileInfo(_logFilePath);
                    if (info.Exists && info.Length > 2 * 1024 * 1024)
                    {
                        var backup = _logFilePath + ".old";
                        File.Delete(backup);
                        File.Move(_logFilePath, backup);
                    }
                }
                catch { }

                File.AppendAllText(_logFilePath, line + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // File IO failure must not disrupt recording
            }
        }

        /// <summary>
        /// Write all in-memory errors to the log file (used before crash exit).
        /// </summary>
        public static void FlushToFile()
        {
            try
            {
                EnsureFileLog();
                if (string.IsNullOrEmpty(_logFilePath))
                    return;

                var sb = new StringBuilder();
                sb.AppendLine($"=== Crash dump {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ===");
                lock (SyncRoot)
                {
                    foreach (var entry in ErrorLog)
                    {
                        sb.AppendLine($"{entry.Timestamp:HH:mm:ss.fff} [{entry.Context}]");
                        sb.AppendLine(entry.FullDetail);
                        sb.AppendLine();
                    }
                }
                sb.AppendLine("=== End crash dump ===");
                File.AppendAllText(_logFilePath, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>
        /// Write a raw message to the log file (for crash handlers).
        /// </summary>
        public static void WriteRaw(string message)
        {
            try
            {
                EnsureFileLog();
                if (string.IsNullOrEmpty(_logFilePath))
                    return;

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                File.AppendAllText(_logFilePath, $"{timestamp} {message}{Environment.NewLine}", Encoding.UTF8);
            }
            catch { }
        }

        public static void Clear()
        {
            lock (SyncRoot) { ErrorLog.Clear(); }
        }

        private class ErrorEntry
        {
            public DateTime Timestamp { get; set; }
            public string Context { get; set; }
            public string FullDetail { get; set; }

            public string Summary =>
                string.IsNullOrEmpty(Context)
                    ? $"{Timestamp:HH:mm:ss} {FullDetail.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0]}"
                    : $"{Timestamp:HH:mm:ss} [{Context}] {FullDetail.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0]}";
        }
    }
}
