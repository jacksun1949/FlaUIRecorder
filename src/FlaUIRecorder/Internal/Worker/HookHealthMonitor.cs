using System;
using System.Timers;

namespace FlaUIRecorder.Internal.Worker
{
    public class HookHealthMonitor : IDisposable
    {
        private const int HealthCheckIntervalMs = 10000;
        private readonly Timer _timer;
        private readonly string _hookName;
        private DateTime _lastEventUtc = DateTime.UtcNow;
        private bool _started;
        private bool _warningFired;

        public event EventHandler<string> StatusChanged;

        public HookHealthMonitor(string hookName)
        {
            _hookName = hookName;
            _timer = new Timer(HealthCheckIntervalMs);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            _started = true;
            _warningFired = false;
            _lastEventUtc = DateTime.UtcNow;
            _timer.Start();
        }

        public void Stop()
        {
            _started = false;
            _timer.Stop();
        }

        public void RecordEvent()
        {
            _lastEventUtc = DateTime.UtcNow;
            _warningFired = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_started || _warningFired)
                return;

            if ((DateTime.UtcNow - _lastEventUtc).TotalMilliseconds >= HealthCheckIntervalMs)
            {
                _warningFired = true;
                StatusChanged?.Invoke(this, $"{_hookName} hook may not be active");
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
