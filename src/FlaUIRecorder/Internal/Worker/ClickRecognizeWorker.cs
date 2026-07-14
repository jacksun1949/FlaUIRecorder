using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using System.Drawing;
using System;
using System.Threading.Tasks;

namespace FlaUIRecorder.Internal.Worker
{
    public class ClickRecognizeWorker : IRecordWorker
    {
        private const int FromPointTimeoutMs = 500;
        private const int DoubleClickThresholdMs = 300;
        private const int DragThresholdPixels = 10;
        private const int IsTargetTimeoutMs = 2000;

        private readonly AutomationBase _automation;
        private readonly HoverElementCache _hoverCache;
        private readonly int _targetProcessId;
        private readonly HookHealthMonitor _healthMonitor;
        private readonly LowLevelMouseHook _mouseHook;

        private AutomationElement _lastClickElement;
        private DateTime _lastClickTime = DateTime.MinValue;
        private bool _isDragging;
        private AutomationElement _dragStartElement;
        private HookPoint _dragStartPoint;
        private bool _pendingSingleClick;
        private AutomationElement _pendingClickElement;

        public event EventHandler<AutomationElement> ElementClicked;
        public event EventHandler<AutomationElement> ElementRightClicked;
        public event EventHandler<AutomationElement> ElementDoubleClicked;
        public event EventHandler<DragActionEventArgs> DragCompleted;
        public event EventHandler<ScrollActionEventArgs> Scrolled;
        public event EventHandler<string> StatusChanged;

        public ClickRecognizeWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)
        {
            _automation = automation;
            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));
            _targetProcessId = targetProcessId;
            _healthMonitor = new HookHealthMonitor("Mouse");
            _healthMonitor.StatusChanged += (s, msg) => StatusChanged?.Invoke(this, msg);

            _mouseHook = new LowLevelMouseHook();
            _mouseHook.MouseEvent += MouseHook_OnMouseEvent;
        }

        public void Dispose()
        {
            Pause();
            _healthMonitor.Dispose();
            _mouseHook.Dispose();
        }

        public void Start()
        {
            try
            {
                _mouseHook.Start();
                _healthMonitor.Start();
                StatusChanged?.Invoke(this, "Mouse hook started");
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "LowLevelMouseHook.Start");
                StatusChanged?.Invoke(this, "Mouse hook failed to start");
            }
        }

        public void Pause()
        {
            FlushPendingClick();
            _isDragging = false;
            try
            {
                _mouseHook.Stop();
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "LowLevelMouseHook.Stop");
            }
            _healthMonitor.Stop();
        }

        private void MouseHook_OnMouseEvent(MouseEventData e)
        {
            try
            {
                _healthMonitor.RecordEvent();

                if (e.Message == MouseMessage.WM_LBUTTONDOWN)
                {
                    var hoveredElement = GetHoveredElement(e.Point);
                    if (hoveredElement == null)
                        return;

                    _isDragging = true;
                    _dragStartElement = hoveredElement;
                    _dragStartPoint = e.Point;
                }
                else if (e.Message == MouseMessage.WM_LBUTTONUP)
                {
                    var hoveredElement = GetHoveredElement(e.Point);

                    if (_isDragging && _dragStartElement != null)
                    {
                        var dx = Math.Abs(e.Point.x - _dragStartPoint.x);
                        var dy = Math.Abs(e.Point.y - _dragStartPoint.y);

                        if (dx > DragThresholdPixels || dy > DragThresholdPixels)
                        {
                            FlushPendingClick();
                            if (hoveredElement != null && !ReferenceEquals(_dragStartElement, hoveredElement))
                            {
                                DragCompleted?.Invoke(this, new DragActionEventArgs
                                {
                                    FromElement = _dragStartElement,
                                    ToElement = hoveredElement
                                });
                            }
                        }
                        else
                        {
                            HandleClick(hoveredElement ?? _dragStartElement);
                        }
                    }

                    _isDragging = false;
                    _dragStartElement = null;
                }
                else if (e.Message == MouseMessage.WM_RBUTTONDOWN)
                {
                    FlushPendingClick();
                    var hoveredElement = GetHoveredElement(e.Point);
                    if (hoveredElement != null)
                        ElementRightClicked?.Invoke(this, hoveredElement);
                }
                else if (e.Message == MouseMessage.WM_MOUSEWHEEL)
                {
                    FlushPendingClick();
                    var hoveredElement = GetHoveredElement(e.Point);
                    if (hoveredElement != null)
                    {
                        // EventHook 1.4.39 does not expose wheel delta; use WHEEL_DELTA default
                        Scrolled?.Invoke(this, new ScrollActionEventArgs
                        {
                            Element = hoveredElement,
                            Delta = 120
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                RecorderErrorLog.RecordError(ex, "ClickRecognizeWorker");
            }
        }

        private void HandleClick(AutomationElement element)
        {
            if (element == null)
                return;

            var now = DateTime.UtcNow;
            if (_lastClickElement != null
                && ReferenceEquals(_lastClickElement, element)
                && (now - _lastClickTime).TotalMilliseconds <= DoubleClickThresholdMs)
            {
                FlushPendingClick();
                ElementDoubleClicked?.Invoke(this, element);
                _lastClickElement = null;
                _lastClickTime = DateTime.MinValue;
                return;
            }

            _pendingSingleClick = true;
            _pendingClickElement = element;
            _lastClickElement = element;
            _lastClickTime = now;

            Task.Delay(DoubleClickThresholdMs + 50).ContinueWith(_ =>
            {
                if (_pendingSingleClick && ReferenceEquals(_pendingClickElement, element))
                    FlushPendingClick();
            });
        }

        private void FlushPendingClick()
        {
            if (!_pendingSingleClick || _pendingClickElement == null)
                return;

            _pendingSingleClick = false;
            ElementClicked?.Invoke(this, _pendingClickElement);
            _pendingClickElement = null;
        }

        private AutomationElement GetHoveredElement(HookPoint point)
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

        private AutomationElement TryFromPoint(HookPoint point)
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
                RecorderErrorLog.RecordError(
                    new TimeoutException($"FromPoint timed out after {FromPointTimeoutMs}ms at ({point.x},{point.y})"),
                    "ClickRecognizeWorker.FromPoint");
                StatusChanged?.Invoke(this, "UIA FromPoint timed out");
                return null;
            }

            if (capturedException != null)
                RecorderErrorLog.RecordError(capturedException, $"ClickRecognizeWorker.FromPoint({point.x},{point.y})");

            return result;
        }

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;

            // UIA ProcessId access is a synchronous COM call that can block indefinitely
            // if the target process is frozen/hung. Use a timeout to prevent deadlocks.
            var processId = 0;
            Exception capturedException = null;

            var task = Task.Run(() =>
            {
                try
                {
                    processId = element.Properties.ProcessId;
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            });

            if (!task.Wait(IsTargetTimeoutMs))
            {
                RecorderErrorLog.RecordError(
                    new TimeoutException($"IsTargetElement.ProcessId timed out after {IsTargetTimeoutMs}ms"),
                    "ClickRecognizeWorker.IsTargetElement");
                return false;
            }

            if (capturedException != null)
            {
                RecorderErrorLog.RecordError(capturedException, "ClickRecognizeWorker.IsTargetElement");
                return false;
            }

            return processId == _targetProcessId;
        }
    }

    public class DragActionEventArgs : EventArgs
    {
        public AutomationElement FromElement { get; set; }
        public AutomationElement ToElement { get; set; }
    }

    public class ScrollActionEventArgs : EventArgs
    {
        public AutomationElement Element { get; set; }
        public int Delta { get; set; }
    }
}
