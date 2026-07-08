using EventHook;
using FlaUI.Core;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.Shapes;
using System;
using System.Threading.Tasks;

namespace FlaUIRecorder.Internal.Worker
{
    public class ClickRecognizeWorker : IRecordWorker
    {
        private const int FromPointTimeoutMs = 500;

        private readonly AutomationBase _automation;
        private readonly HoverElementCache _hoverCache;
        private readonly int _targetProcessId;

        public event EventHandler<AutomationElement> ElementClicked;
        public event EventHandler<AutomationElement> ElementRightClicked;
        public event EventHandler<string> StatusChanged;

        public ClickRecognizeWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)
        {
            _automation = automation;
            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));
            _targetProcessId = targetProcessId;

            MouseWatcher.OnMouseInput += MouseWatcher_OnMouseInput;
        }

        public void Dispose()
        {
            Pause();
            MouseWatcher.OnMouseInput -= MouseWatcher_OnMouseInput;
        }

        public void Start()
        {
            try
            {
                MouseWatcher.Start();
                StatusChanged?.Invoke(this, "Mouse hook started");
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "MouseWatcher.Start");
                StatusChanged?.Invoke(this, "Mouse hook failed to start");
            }
        }

        public void Pause()
        {
            try
            {
                MouseWatcher.Stop();
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "MouseWatcher.Stop");
            }
        }

        private void MouseWatcher_OnMouseInput(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Message == EventHook.Hooks.MouseMessages.WM_LBUTTONDOWN)
                {
                    var hoveredElement = GetHoveredElement(e.Point);
                    if (hoveredElement != null)
                        ElementClicked?.Invoke(this, hoveredElement);
                }
                else if (e.Message == EventHook.Hooks.MouseMessages.WM_RBUTTONDOWN)
                {
                    var hoveredElement = GetHoveredElement(e.Point);
                    if (hoveredElement != null)
                        ElementRightClicked?.Invoke(this, hoveredElement);
                }
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "ClickRecognizeWorker");
            }
        }

        private AutomationElement GetHoveredElement(EventHook.Hooks.POINT point)
        {
            if (_hoverCache.TryGetNear(point.x, point.y, out var cachedElement) && IsTargetElement(cachedElement))
                return cachedElement;

            var hoveredElement = TryFromPoint(point);
            if (IsTargetElement(hoveredElement))
            {
                _hoverCache.Update(point.x, point.y, hoveredElement);
                return hoveredElement;
            }

            if (_hoverCache.TryGetWithin(point.x, point.y, HoverElementCache.FallbackDistancePixels, out cachedElement)
                && IsTargetElement(cachedElement))
                return cachedElement;

            return null;
        }

        private AutomationElement TryFromPoint(EventHook.Hooks.POINT point)
        {
            AutomationElement result = null;
            Exception capturedException = null;

            var task = Task.Run(() =>
            {
                try
                {
                    lock (_hoverCache.SyncRoot)
                    {
                        result = _automation.FromPoint(new Point(point.x, point.y));
                    }
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            });

            if (!task.Wait(FromPointTimeoutMs))
            {
                RecorderErrorLog.RecordError(new TimeoutException($"FromPoint timed out after {FromPointTimeoutMs}ms"), "ClickRecognizeWorker.FromPoint");
                StatusChanged?.Invoke(this, "UIA FromPoint timed out");
                return null;
            }

            if (capturedException != null)
                RecorderErrorLog.RecordError(capturedException, "ClickRecognizeWorker.FromPoint");

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
                RecorderErrorLog.RecordError(ex, "ClickRecognizeWorker.IsTargetElement");
                return false;
            }
        }
    }
}
