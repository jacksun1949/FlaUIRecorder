using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Input;
using FlaUI.Core.Shapes;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace FlaUIRecorder.Internal.Worker
{
    public class HoverWorker : IRecordWorker
    {
        private const int PollIntervalMs = 120;
        private const int HighlightDurationMs = 350;
        private const int FromPointTimeoutMs = 500;

        private readonly AutomationBase _automation;
        private readonly Timer _dispatcherTimer;
        private readonly HoverElementCache _hoverCache;
        private AutomationElement _currentHoveredElement;
        private readonly int _targetProcessId;
        private int _lastMouseX = int.MinValue;
        private int _lastMouseY = int.MinValue;

        public event EventHandler<AutomationElement> ElementHovered;
        public event EventHandler<string> StatusChanged;

        public AutomationElement CurrentHoveredElement => _currentHoveredElement;

        public HoverWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)
        {
            _automation = automation;
            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));
            _dispatcherTimer = new Timer(PollIntervalMs);
            _dispatcherTimer.Elapsed += DispatcherTimerTick;
            _targetProcessId = targetProcessId;
        }

        public void Dispose()
        {
            Pause();
        }

        public void Start()
        {
            _currentHoveredElement = null;
            _lastMouseX = int.MinValue;
            _lastMouseY = int.MinValue;
            _dispatcherTimer.Start();
        }

        public void Pause()
        {
            _currentHoveredElement = null;
            _lastMouseX = int.MinValue;
            _lastMouseY = int.MinValue;
            _hoverCache.Clear();
            _dispatcherTimer.Stop();
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            var screenPos = Mouse.Position;
            var mouseX = (int)screenPos.X;
            var mouseY = (int)screenPos.Y;

            if (mouseX == _lastMouseX && mouseY == _lastMouseY)
                return;

            _lastMouseX = mouseX;
            _lastMouseY = mouseY;

            try
            {
                var hoveredElement = FromPointWithTimeout(screenPos);
                if (!IsTargetElement(hoveredElement))
                {
                    _currentHoveredElement = null;
                    return;
                }

                _hoverCache.Update(mouseX, mouseY, hoveredElement);

                if (!Equals(_currentHoveredElement, hoveredElement))
                {
                    _currentHoveredElement = hoveredElement;
                    ElementHovered?.Invoke(this, hoveredElement);
                }

                HighlightElement(hoveredElement);
            }
            catch (Exception ex)
            {
                _currentHoveredElement = null;
                RecorderErrorLog.RecordError(ex, "HoverWorker");
            }
        }

        private AutomationElement FromPointWithTimeout(Point screenPos)
        {
            AutomationElement result = null;
            Exception capturedException = null;

            var task = Task.Run(() =>
            {
                try
                {
                    lock (_hoverCache.SyncRoot)
                    {
                        result = _automation.FromPoint(screenPos);
                    }
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            });

            if (!task.Wait(FromPointTimeoutMs))
            {
                RecorderErrorLog.RecordError(new TimeoutException($"FromPoint timed out after {FromPointTimeoutMs}ms"), "HoverWorker.FromPoint");
                StatusChanged?.Invoke(this, "UIA FromPoint timed out");
                return null;
            }

            if (capturedException != null)
                RecorderErrorLog.RecordError(capturedException, "HoverWorker.FromPoint");

            return result;
        }

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;

            try
            {
                return element.Properties.ProcessId == _targetProcessId;
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "HoverWorker.IsTargetElement");
                return false;
            }
        }

        public static void HighlightElement(AutomationElement automationElement)
        {
            if (automationElement == null)
                return;

            Task.Run(() =>
            {
                try
                {
                    automationElement.DrawHighlight(false, System.Drawing.Color.Red, HighlightDurationMs);
                }
                catch (Exception ex)
                {
                    RecorderErrorLog.RecordError(ex, "HoverWorker.HighlightElement");
                }
            });
        }
    }
}
