using System;
using System.IO;
using System.Timers;
using System.Web.Script.Serialization;

namespace FlaUIRecorder.Internal
{
    public class RecordingAutosave : IDisposable
    {
        private const int AutosaveIntervalMs = 30000;
        private const string AutosaveExtension = ".urp.autosave";

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer
        {
            MaxJsonLength = int.MaxValue,
            RecursionLimit = 128
        };

        private readonly Timer _timer;
        private RecorderProject _project;
        private Func<System.Collections.Generic.List<RecordedAction>> _getActions;

        public RecordingAutosave()
        {
            _timer = new Timer(AutosaveIntervalMs);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start(RecorderProject project, Func<System.Collections.Generic.List<RecordedAction>> getActions)
        {
            _project = project;
            _getActions = getActions;
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
                if (_project == null || string.IsNullOrEmpty(_project.FileName))
                    return;

                var autosavePath = _project.FileName + AutosaveExtension;
                var snapshot = Serializer.Serialize(_project);
                File.WriteAllText(autosavePath, snapshot);
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
                project = Serializer.Deserialize<RecorderProject>(json);
                return project != null;
            }
            catch
            {
                return false;
            }
        }

        public static void DeleteAutosave(string projectFileName)
        {
            var path = GetAutosavePath(projectFileName);
            if (path != null && File.Exists(path))
            {
                try { File.Delete(path); } catch { }
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
