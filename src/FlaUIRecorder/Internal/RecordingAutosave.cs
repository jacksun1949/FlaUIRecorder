using System;
using System.IO;
using System.Text.Json;
using System.Timers;

namespace FlaUIRecorder.Internal
{
    public class RecordingAutosave : IDisposable
    {
        private const int AutosaveIntervalMs = 30000;
        private const string AutosaveExtension = ".urp.autosave";

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private readonly Timer _timer;
        private RecorderProject _project;
        private Func<System.Collections.Generic.List<RecordedAction>> _getActions;
        private readonly object _lock = new object();

        public RecordingAutosave()
        {
            _timer = new Timer(AutosaveIntervalMs);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start(RecorderProject project, Func<System.Collections.Generic.List<RecordedAction>> getActions)
        {
            lock (_lock)
            {
                _project = project;
                _getActions = getActions;
            }
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RecorderProject projectSnapshot;
                lock (_lock)
                {
                    if (_project == null || string.IsNullOrEmpty(_project.FileName))
                        return;

                    projectSnapshot = _project;
                }

                var autosavePath = projectSnapshot.FileName + AutosaveExtension;
                var json = JsonSerializer.Serialize(projectSnapshot, Options);
                File.WriteAllText(autosavePath, json);
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "RecordingAutosave");
            }
        }

        public static string GetAutosavePath(string projectFileName)
        {
            return string.IsNullOrEmpty(projectFileName) ? null : projectFileName + AutosaveExtension;
        }

        public static bool TryLoadAutosave(string projectFileName, out RecorderProject project)
        {
            project = null;
            var path = GetAutosavePath(projectFileName);
            if (path == null || !File.Exists(path))
                return false;

            try
            {
                var json = File.ReadAllText(path);
                project = JsonSerializer.Deserialize<RecorderProject>(json, Options);
                return project != null;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "RecordingAutosave.TryLoadAutosave");
                return false;
            }
        }

        public static void DeleteAutosave(string projectFileName)
        {
            var path = GetAutosavePath(projectFileName);
            if (path != null && File.Exists(path))
            {
                try { File.Delete(path); }
                catch (Exception ex) { RecorderErrorLog.RecordError(ex, "RecordingAutosave.DeleteAutosave"); }
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
