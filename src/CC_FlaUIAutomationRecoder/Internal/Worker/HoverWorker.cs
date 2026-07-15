using FlaUI.Core;

using FlaUI.Core.AutomationElements;

using FlaUI.Core.Input;

using System.Drawing;

using System;

using System.Threading.Tasks;

using System.Timers;

using System.Windows.Forms;



namespace CC_FlaUIAutomationRecoder.Internal.Worker

{

    public class HoverWorker : IRecordWorker

    {

        private const int PollIntervalMs = 120;

        private const int HighlightDurationMs = 350;

        private const int FromPointTimeoutMs = 500;

        private const int HoverStayThresholdMs = 1000;



        private readonly AutomationBase _automation;

        private readonly System.Timers.Timer _dispatcherTimer;

        private readonly HoverElementCache _hoverCache;

        private AutomationElement _currentHoveredElement;

        private readonly int _targetProcessId;

        private int _lastMouseX = int.MinValue;

        private int _lastMouseY = int.MinValue;

        private DateTime _hoverStartUtc = DateTime.MinValue;

        private AutomationElement _hoverStayElement;

        private bool _hoverStayEmitted;



        public event EventHandler<AutomationElement> ElementHovered;

        public event EventHandler<HoverStayEventArgs> HoverStayCompleted;

        public event EventHandler<string> StatusChanged;



        public AutomationElement CurrentHoveredElement => _currentHoveredElement;



        public HoverWorker(AutomationBase automation, int targetProcessId, HoverElementCache hoverCache)

        {

            _automation = automation;

            _hoverCache = hoverCache ?? throw new ArgumentNullException(nameof(hoverCache));

            _dispatcherTimer = new System.Timers.Timer(PollIntervalMs);

            _dispatcherTimer.Elapsed += DispatcherTimerTick;

            _targetProcessId = targetProcessId;

        }



        public void Dispose()

        {

            Pause();
            _dispatcherTimer.Dispose();

        }



        public void Start()

        {

            _currentHoveredElement = null;

            _lastMouseX = int.MinValue;

            _lastMouseY = int.MinValue;

            _hoverStartUtc = DateTime.MinValue;

            _hoverStayElement = null;

            _hoverStayEmitted = false;

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



            // Use the screen containing the mouse for DPI-aware coordinates

            var screen = Screen.FromPoint(new System.Drawing.Point(mouseX, mouseY));



            if (mouseX == _lastMouseX && mouseY == _lastMouseY)

            {

                CheckHoverStay();

                return;

            }



            _lastMouseX = mouseX;

            _lastMouseY = mouseY;

            _hoverStartUtc = DateTime.UtcNow;

            _hoverStayEmitted = false;



            try

            {

                var hoveredElement = FromPointWithTimeout(screenPos);

                if (!IsTargetElement(hoveredElement))

                {

                    _currentHoveredElement = null;

                    _hoverStayElement = null;

                    return;

                }



                _hoverCache.Update(mouseX, mouseY, hoveredElement);



                if (!ReferenceEquals(_hoverStayElement, hoveredElement))

                {

                    _hoverStayElement = hoveredElement;

                    _hoverStartUtc = DateTime.UtcNow;

                    _hoverStayEmitted = false;

                }



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



        private void CheckHoverStay()

        {

            if (_hoverStayElement == null || _hoverStayEmitted)

                return;



            var duration = (int)(DateTime.UtcNow - _hoverStartUtc).TotalMilliseconds;

            if (duration >= HoverStayThresholdMs)

            {

                _hoverStayEmitted = true;

                HoverStayCompleted?.Invoke(this, new HoverStayEventArgs

                {

                    Element = _hoverStayElement,

                    DurationMs = duration

                });

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



        private const int IsTargetTimeoutMs = 2000;

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;

            // UIA ProcessId access is a synchronous COM call that can block indefinitely
            // if the target process is frozen/hung. Use a timeout to prevent deadlocks.
            var processId = 0;
            Exception capturedException = null;

            var task = System.Threading.Tasks.Task.Run(() =>
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
                    new TimeoutException($"HoverWorker.IsTargetElement timed out after {IsTargetTimeoutMs}ms"),
                    "HoverWorker.IsTargetElement");
                return false;
            }

            if (capturedException != null)
            {
                RecorderErrorLog.RecordError(capturedException, "HoverWorker.IsTargetElement");
                return false;
            }

            return processId == _targetProcessId;
        }



        public static void HighlightElement(AutomationElement automationElement)

        {

            if (automationElement == null)

                return;



            Task.Run(() =>

            {

                try

                {

                    automationElement.DrawHighlight(false, System.Drawing.Color.Red, TimeSpan.FromMilliseconds(HighlightDurationMs));

                }

                catch (Exception ex)

                {

                    RecorderErrorLog.RecordError(ex, "HoverWorker.HighlightElement");

                }

            });

        }

    }



    public class HoverStayEventArgs : EventArgs

    {

        public AutomationElement Element { get; set; }

        public int DurationMs { get; set; }

    }

}


